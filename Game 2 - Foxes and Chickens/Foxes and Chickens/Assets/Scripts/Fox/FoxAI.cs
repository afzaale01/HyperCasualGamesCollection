using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Require that we have a pathfinding component
//attached to this object so that we can pathfind
[RequireComponent(typeof(AIPathfinding))]
public class FoxAI : Character
{
    [SerializeField]
    private GameObject playerObject; //Player Object
    private bool playerSeen; //Can we see the player?
    private Vector3 lastKnownPlayerPos; //What is the last known position of the player

    //Position we are working towards
    private Vector3 targetPos;
    //Store the target position from the previous update, so that we 
    //don't have to refind a route if it is the same as the new
    //target pos
    private Vector3 prevTargetPos;
    //List of the positions (from GameObjects) from the most recent pathfind
    List<GameObject> lastPathfindResults;

    //Settings for a fox turn



    //Pathfinding Component
    private AIPathfinding pathfinder;

    //Maximum distance that the fox can see the player
    //from
    private const float maxViewDist = 5.0f;
    //Layer to ignore when raycasting
    private const int raycastIgnoreLayer = 8;
    private const int notPlayerLayerMask = ~(1 << raycastIgnoreLayer);

    private enum AI_STATE
    {
        AI_STATE_SEEK_TO_PLAYER,
        AI_STATE_SEEK_TO_LAST_POS,
        AI_STATE_WANDER,

        AI_STATE_COUNT //Number of states
    }
    private AI_STATE currentAIState;


    private void Start()
    {
        //Init current state to wander, as we don't know if we 
        //can see the player when we start
        currentAIState = AI_STATE.AI_STATE_WANDER;

        //Init Target Pos and prev target to be our current position
        prevTargetPos = targetPos = transform.position;

        //Get required pathfinding component
        pathfinder = GetComponent<AIPathfinding>();

        //Find the player object if there is not one already assigned
        if(playerObject == null)
        {
            playerObject = FindObjectOfType<PlayerData>().gameObject;
        }
    }


    /// <summary>
    /// Work out the new state for the AI to be in
    /// </summary>
    /// <returns>New State</returns>
    private void UpdateAI(GameStateManager.GameTurn newTurn)
    {
        //If it is not our turn then return
        if(newTurn != GameStateManager.GameTurn.TURN_AI)
        {
            return;
        }

        //Null Check Player and early out
        if (!playerObject)
        {
            return;
        }

        //Get a new state, based on current conditions
        currentAIState = GetNewAIState();

        //Set the prev target pos to the current pos,
        //so we can check if the target pos hasn't moved
        prevTargetPos = targetPos;

        switch (currentAIState)
        {
            case AI_STATE.AI_STATE_SEEK_TO_PLAYER:
                {
                    //Set our target position to be the player
                    targetPos = playerObject.transform.position;
                    //Update the last known player position, set it to our y pos
                    //as neither object moves in the y direction
                    lastKnownPlayerPos = new Vector3(targetPos.x, transform.position.y, targetPos.z);
                    break;
                }
            case AI_STATE.AI_STATE_SEEK_TO_LAST_POS:
                {
                    //Set the target pos to the players
                    //last know position
                    targetPos = lastKnownPlayerPos;

                    break;
                }
            case AI_STATE.AI_STATE_WANDER:
                {
                    //Find a point on the map that we should
                    //go to, then set it as our target pos
                    targetPos = GetWanderTargetPos();
                    break;
                }
        }

        //If the target pos is not the same as the previous
        //and it is not the current pos
        //target pos find a route to the target
        if(targetPos != prevTargetPos && targetPos != transform.position)
        {
            lastPathfindResults = pathfinder.GetAStarPath(transform.position, targetPos);
        }

        //Check that we have a path to follow
        if (lastPathfindResults.Count > 0)
        {
            //Get the next pos from the list and keep our y the same
            //because the grid is flat to the x/z plane
            Vector3 nextPos = lastPathfindResults[0].transform.position;
            nextPos.y = transform.position.y;

            //Rotate and move towards next position
            transform.LookAt(nextPos);

            //Check that the tile at the next pos is not occupied by the player
            WalkableTile nextTile = lastPathfindResults[0].GetComponentInChildren<WalkableTile>();
            if (nextTile)
            {
                if (!nextTile.IsOccupied || nextTile.Occupier.GetComponent<PlayerMovement>())
                {
                    //Attempt to reserve the space and only move if we do this succcessfully
                    if (nextTile.AttemptToReserve(this.gameObject))
                    {
                        StartCoroutine(MoveOverTime(nextPos, turnDuration));
                    }
                }
            }

            //Remove the pos from the list so we don't 
            //visit it again
            lastPathfindResults.RemoveAt(0);
        }

        //Debug Test for State
        Debug.Log("FOX :: " + currentAIState.ToString());

        //End the turn after turn timer is up - call event
        StartCoroutine(EndTurnAfterTime(GameStateManager.GameTurn.TURN_AI, turnDuration));

    }


    /// <summary>
    /// Get the new state for this ai based on the 
    /// current world conditions
    /// </summary>
    /// <returns></returns>
    private AI_STATE GetNewAIState()
    {
        //Final state to return, default to returing the 
        //current state
        AI_STATE returnState = currentAIState;

        //Check for LOS
        playerSeen = CanSeePlayer();

        //If we can see the player then instantly 
        //change to chase the AI
        if (playerSeen)
        {
            return AI_STATE.AI_STATE_SEEK_TO_PLAYER;
        }
        else
        {
            /*
             * Go through states if we are activly persuing the player
             * or the last know position and update if we should wander or
             * try to get to the players last know position
             */
            switch (currentAIState)
            {
                case AI_STATE.AI_STATE_SEEK_TO_PLAYER:
                {
                    //We haven't seen the player this turn and are currently seeking the player
                    //then we should now move to the last known pos
                    returnState = AI_STATE.AI_STATE_SEEK_TO_LAST_POS;
                    break;
                }
                case AI_STATE.AI_STATE_SEEK_TO_LAST_POS:
                {

                    //If we are at the players last pos then switch to wander
                    if (transform.position == lastKnownPlayerPos)
                    {
                        returnState = AI_STATE.AI_STATE_WANDER;
                    }
                    break;
                }
                case AI_STATE.AI_STATE_WANDER:
                {
                    //Don't need to change anything with wander just break
                    break;
                }
            }
        }

        //No state change return the current state
        return returnState;
    }

    /// <summary>
    /// Check if we have line of sight on the player
    /// </summary>
    /// <returns>If the player can be seen by this object</returns>
    private bool CanSeePlayer()
    {
        //Null check player
        if (!playerObject)
        {
            return false;
        }

        //Get player direction and distance
        Vector3 playerPosition = playerObject.transform.position;
        Vector3 foxPosition = transform.position;
        Vector3 playerDirection = (playerPosition - foxPosition).normalized;
        float playerDistance = Vector3.Distance(playerPosition, foxPosition);

        //If the distance to the player is greater than the max
        //view distance then return false
        if(playerDistance > maxViewDist)
        {
            return false;
        }

        //Raycast to see if anything is in the way of us seeing the player
        //If we hit anything then we cannot see the player
        //We will not hit the player because it is on a different physics layer
        //We are ignoring triggers so will not hit the triggers above floor tiles
        RaycastHit rayHit;
        if (Physics.Raycast(transform.position, playerDirection, out rayHit, playerDistance, notPlayerLayerMask, QueryTriggerInteraction.Ignore))
        {
            //Something is blocking our vision - can't see player
            return false;
        }

        //Nothing intercepted us when trying to look for the player
        //therefore we can see the player 
        return true;
    }

    /// <summary>
    /// Gets a position within the world for the Fox AI to wander to
    /// </summary>
    /// <returns>Target Position to Wander to</returns>
    private Vector3 GetWanderTargetPos()
    {
        //Directions array for all directions that we can move in
        Vector3[] possibleDirections = { transform.forward, -transform.right, transform.right, -transform.forward };
        //List for directions that are free to move in
        List<Vector3> moveableDirections = new List<Vector3>();

        
        //For each direction, check if there is a walkable tile in that direction
        foreach (Vector3 direction in possibleDirections)
        {
            RaycastHit rayhit;
            //Cast out and see if the tile 1 space in the given direction is free
            if(Physics.Raycast(transform.position, direction, out rayhit, GameStateManager.tileMoveAmount, notPlayerLayerMask, QueryTriggerInteraction.Collide))
            {
                //Check that we hit a walkable tile
                if (rayhit.collider.gameObject.GetComponentInChildren<WalkableTile>())
                {
                    //Add to the list as a direction that we can move in
                    moveableDirections.Add(direction);
                }
            }
        }

        //Check that we have a direction to move in
        if(moveableDirections.Count > 0)
        {
            //Choose a random direction from the direction
            //list
            int chosenIndex = Random.Range(0, moveableDirections.Count);
            Vector3 chosenDirection = moveableDirections[chosenIndex];

            //Return our current position + the chosen direction
            return transform.position + chosenDirection;
        }

        //Default Return - we have failed to find a position to move to
        //return the current position so that we don't move
        return transform.position;

    }

    /// <summary>
    /// Destory this Fox
    /// </summary>
    private void DestroyFox()
    {
        Destroy(gameObject);
    }

    #region Event Subs/UnSubs
    private void OnEnable()
    {
        GameStateManager.GameTurnStarted += UpdateAI;

        //If a fox exists when the level start event is called,
        //destroy it because we have most likley restarting
        GameStateManager.LevelStarted += DestroyFox;
    }

    private void OnDisable()
    {
        GameStateManager.GameTurnStarted -= UpdateAI;
        GameStateManager.LevelStarted -= DestroyFox;
    }
    #endregion
}
