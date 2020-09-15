using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lewis.TouchExtensions;

public class PlayerMovement : Character
{
    //Bool for if the player is allowed to move
    //(i.e paused or not)
    private bool allowMovement = true;

    // Update is called once per frame
    private void Update()
    {
        if (!AllowTurnInteraction())
        {
            return;
        }

        //Check that we have a valid touch
        if (!TouchInput.TouchIsValid(TouchDefines.movementTouchID))
        {
            return;
        }

        //Get touch
        Touch moveTouch = Input.GetTouch(TouchDefines.movementTouchID);
            
        //Only move once the touch has ended
        if (moveTouch.phase != TouchPhase.Ended)
        {
            return;
        }

        //Get the direction that the touch has moved
        Vector2 swipeDirection = TouchInput.GetMainSwipeDirection(moveTouch);
        Vector3 moveDirection = new Vector3(swipeDirection.x, 0, swipeDirection.y);

        //Check if where we want to move to has a tile that we can go to
        RaycastHit rayhit;
        if (Physics.Raycast(transform.position, moveDirection, out rayhit, GameStateManager.tileMoveAmount))
        {
            //Get if we have hit a tile the move to that tiles center
            WalkableTile hitTile = rayhit.collider.gameObject.GetComponent<WalkableTile>();
            if (hitTile){

                //Rotate and move to tile
                gameObject.transform.LookAt(hitTile.transform);
                StartCoroutine(MoveOverTime(hitTile.transform.position, turnDuration));

                //End the turn after time

                StartCoroutine(EndTurnAfterTime(GameStateManager.GameTurn.TURN_PLAYER, turnDuration));
            }
        }
    }

    /// <summary>
    /// Checks if it is our turn and that interaction is allowed
    /// (i.e we are not moving after we just swiped or paused)
    /// </summary>
    /// <returns>If turn interaction is allowed</returns>
    private bool AllowTurnInteraction()
    {
        if (allowMovement && !TurnInteractionEnded &&
            GameStateManager.CurrentTurn == GameStateManager.GameTurn.TURN_PLAYER)
        {
            return true;
        }

        return false;
    }


    /// <summary>
    /// Enable the player from being able to move
    /// </summary>
    private void EnableMovement()
    {
        allowMovement = true;
    }

    /// <summary>
    /// Disable the Player from being able to move
    /// </summary>
    private void DisableMovement()
    {
        allowMovement = false;
    }

    #region Event Subs/UnSubs
    private void OnEnable()
    {
        GameStateManager.LevelStarted += EnableMovement;
        GameStateManager.LevelUnPaused += EnableMovement;

        GameStateManager.LevelPaused += DisableMovement;
        GameStateManager.LevelFailed += DisableMovement;
        GameStateManager.LevelCompleted += DisableMovement;
    }
    private void OnDisable()
    {
        GameStateManager.LevelStarted -= EnableMovement;
        GameStateManager.LevelUnPaused -= EnableMovement;

        GameStateManager.LevelPaused -= DisableMovement;
        GameStateManager.LevelFailed -= DisableMovement;
        GameStateManager.LevelCompleted -= DisableMovement;
    }
    #endregion
}
