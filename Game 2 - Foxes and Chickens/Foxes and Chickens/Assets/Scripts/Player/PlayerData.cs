using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{

    //Event for when the player is killed
    public delegate void PlayerHealthEvent();
    public static event PlayerHealthEvent PlayerKilled;

    //Check for collision with a fox and thus triggering the player killed event
    private void OnTriggerEnter(Collider collision)
    {
        //Check if we have collided with a fox
        if (collision.gameObject.GetComponent<FoxAI>())
        {
            //Trigger Player Killed Event
            PlayerKilled?.Invoke();

            //We have collided with a fox and have been caught
            //Call game over from the game state manager
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Destroy the Player Object
    /// </summary>
    private void DestoryPlayer()
    {
        Destroy(gameObject);
    }

    #region Events Subs/Unsubs
    private void OnEnable()
    {
        //If a player exists when the level start event is called,
        //destroy it because we have most likley restarting
        GameStateManager.LevelStarted += DestoryPlayer;
    }
    private void OnDisable()
    {
        GameStateManager.LevelStarted -= DestoryPlayer;
    }
    #endregion

}
