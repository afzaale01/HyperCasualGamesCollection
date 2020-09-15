using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to deal with functionality for objects that have been spawned
/// (i.e sleeping the rigidbody)
/// </summary>
public class SpawnedObject : MonoBehaviour
{

    private Rigidbody objectRB;

    private void Awake()
    {
        //Get Rigidbody on this object
        objectRB = GetComponent<Rigidbody>();
    }


    #region Event Subs/UnSubs

    private void OnEnable()
    {
        //Sleep the RB when we pause and unsleep it when 
        //we unpause
        if (objectRB)
        {
            GameStateManager.LevelPaused += objectRB.Sleep;
            GameStateManager.LevelCompleted += objectRB.Sleep;
            GameStateManager.LevelUnPaused += objectRB.WakeUp;
        }
    }
    private void OnDisable()
    {
        if (objectRB)
        {

            //Sleep the RB when we pause and unsleep it when 
            //we unpause
            GameStateManager.LevelPaused -= objectRB.Sleep;
            GameStateManager.LevelCompleted -= objectRB.Sleep;
            GameStateManager.LevelUnPaused -= objectRB.WakeUp;
        }
    }

    #endregion
}
