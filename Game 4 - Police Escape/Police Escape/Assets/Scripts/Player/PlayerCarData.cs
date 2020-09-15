using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerCarData : CarData
{
    //Event for player update/killed
    public delegate void PlayerKilledEvent();
    public static event PlayerKilledEvent PlayerKilled;

    //Event for when player health is updated
    public delegate void PlayerHealthEvent(float newHealth);
    public static event PlayerHealthEvent UpdatePlayerHealth;

    private PlayerMovement playerControl;

    private new void Start()
    {
        //Call base function
        base.Start();

        //Get our movement script
        playerControl = GetComponent<PlayerMovement>();
    }

    protected override void UpdateCarState()
    {
        //Call base function
        base.UpdateCarState();

        //Trigger Event - update UI and what not
        UpdatePlayerHealth?.Invoke(CarHealth);
    }

    /// <summary>
    /// Disable the Player Car
    /// </summary>
    protected override void DisableCar()
    {
        if (currentState == CarState.Enabled)
        {
            //Call Event
            PlayerKilled?.Invoke();


            //Call base function
            base.DisableCar();

            //Disable our movement
            if (playerControl)
            {
                playerControl.enabled = false;
            }

            if (controllingCar)
            {
                controllingCar.CarAccelerateInput = 0f;
                controllingCar.CarSteerInput = 0f;
            }

            //Disable this data
            this.enabled = false;
        }
    }
}
