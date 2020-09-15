using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to wrap player movement inputs
/// </summary>
[RequireComponent(typeof(CarPhysics))]
public class PlayerMovement : MonoBehaviour
{

    //Joystick that we should get player input from
    [SerializeField]
    private Joystick controlJoystick;

    //Car that the player is controlling
    private CarPhysics controllingCar;

    private void Start()
    {
        //Get out car physics
        controllingCar = GetComponent<CarPhysics>();
        if (controllingCar == null)
        {
            Debug.LogWarning("Player Movement does not have a CAR assigned");
        }
    }

    private void Update()
    {
        //Set the inputs from our joystick to the car physics update
        controllingCar.CarAccelerateInput = controlJoystick.Vertical;
        controllingCar.CarSteerInput = controlJoystick.Horizontal;
    }
}
