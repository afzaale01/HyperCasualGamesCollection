using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{

    [SerializeField] private Transform playerCreatePoint;

    public delegate void PlayerEvent();
    //Event for player Killed
    public static event PlayerEvent PlayerKilled;

    private void Awake()
    {
        //Load the model of the player at the create point
        if (GameStateManager.PlayerSelectedCar && playerCreatePoint)
        {
            GameObject playerCar = Instantiate(GameStateManager.PlayerSelectedCar.model, playerCreatePoint, false);
            playerCar.transform.localPosition = Vector3.zero;
        }
        else
        {
            //Display Warning that we could not load the player model
            Debug.LogWarning("Could not spawn player car as the either the player selected car or player create point were null");
        }
    }

    private void Start()
    {
        //Trigger Level Started when the player car is spawner
        GameStateManager.StartLevel();
    }

    //Collision with crater
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<Crater>())
        {
            //Call Player Killed Event
            PlayerKilled?.Invoke();
            Destroy(gameObject);
        }
    }
}
