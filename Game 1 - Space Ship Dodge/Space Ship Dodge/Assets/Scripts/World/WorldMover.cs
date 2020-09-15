using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMover : MonoBehaviour
{

    //Game camera so that we can delete/deactiate objects if they are outside its FOV
    [SerializeField]
    private Camera gameCamera;

    //List of all of the objects in the world that should be moved
    private List<GameObject> worldObjects;

    //Speed the world should move
    [SerializeField]
    private Vector3 moveSpeed = Vector3.zero;
    private Vector3 currentOffset; //Current offset of all objects from thier start pos
    private bool moveWorld = true;

    //If the game should unload the level once it is passed the 
    //camera - useful for optimisation on mobile while allowing
    //debugging
    [SerializeField]
    private bool unloadOutsideCamera = true;

    // Start is called before the first frame update
    void Start()
    {
        //Get all of the children of this object and assign that to the world object
        //that we are moving
        worldObjects = new List<GameObject>();
        for(int i = 0; i < transform.childCount; i++)
        {
            worldObjects.Add(gameObject.transform.GetChild(i).gameObject);
        }

        //Initalise current offset
        currentOffset = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //Check that we should be moving the world
        //and move the world
        if (moveWorld)
        {
            MoveWorldObjects();
        }

    }


    /// <summary>
    /// Moves all of the objects within the world around the player
    /// </summary>
    private void MoveWorldObjects()
    {
        if (worldObjects != null)
        {
            //Loop through all moveable objects and move them
            for (int i = 0; i < worldObjects.Count; i++)
            {
                GameObject currentObject = worldObjects[i];
                if (currentObject != null)
                {
                    //Move Object
                    Vector3 movement = moveSpeed * Time.deltaTime;
                    currentObject.transform.position += movement;
                    currentOffset += movement;
                }
            }
        }
    }

    /// <summary>
    /// Enable world objects moving
    /// </summary>
    private void EnableWorldMovement()
    {
        moveWorld = true;
    }

    /// <summary>
    /// Disable world objects moving
    /// </summary>
    private void DisableWorldMovement()
    {
        moveWorld = false;
    }

    /// <summary>
    /// Add a new object to move through the world
    /// </summary>
    /// <param name="obj"></param>
    private void AddNewMovingObject(GameObject obj)
    {
        if (worldObjects != null && obj != null)
        {
            worldObjects.Add(obj);
        }
    }

    /// <summary>
    /// Remove an object and stop it coming through the world
    /// </summary>
    /// <param name="obj"></param>
    private void RemoveMovingObject(GameObject obj)
    {
        if(worldObjects != null)
        {
            if (worldObjects.Contains(obj))
            {
                worldObjects.Remove(obj);
            }
        }
    }


    #region Event Subs/UnSubs
    private void OnEnable()
    {
        //Game Events
        GameStateManager.LevelPaused += DisableWorldMovement;
        GameStateManager.LevelUnPaused += EnableWorldMovement;
        GameStateManager.LevelFailed += DisableWorldMovement;
        GameStateManager.LevelCompleted += DisableWorldMovement;

        //Adding/Removing objects 
        ExplosionPiece.PieceCreated += AddNewMovingObject;
        ExplosionPiece.PieceDestroyed += RemoveMovingObject;
    }

    private void OnDisable()
    {
        //Game Events
        GameStateManager.LevelPaused -= DisableWorldMovement;
        GameStateManager.LevelUnPaused -= EnableWorldMovement;
        GameStateManager.LevelFailed -= DisableWorldMovement;
        GameStateManager.LevelCompleted -= DisableWorldMovement;


        //Adding/Removing objects 
        ExplosionPiece.PieceCreated -= AddNewMovingObject;
        ExplosionPiece.PieceDestroyed -= RemoveMovingObject;
    }

    #endregion
}
