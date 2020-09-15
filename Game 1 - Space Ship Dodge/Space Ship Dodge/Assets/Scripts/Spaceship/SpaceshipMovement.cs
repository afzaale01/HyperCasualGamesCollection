using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lewis.TouchExtensions;
using Lewis.MathUtils;
using Lewis.Score;
using System;

/// <summary>
/// Class to deal with ship movement
/// </summary>
public class SpaceshipMovement : MonoBehaviour
{
    //Target Position and Velocity
    private Vector3 currentVelocity;
    private Vector3 targetPostion; //Target Postion to interpolate to
    private Vector3 startPosition;

    //Movement dampening - appox time to get between current and target pos
    //Lower value = faster time to target pos
    [SerializeField]
    private float movementDampening = 0.1f;
    private float inputSensitivity = 1.0f;

    //Minimum and maximum y values so that we don't go off the screen
    [Header("Movement Constraints")]
    [SerializeField]
    private Vector3 minPosition = Vector3.negativeInfinity;
    [SerializeField]
    private Vector3 maxPosition = Vector3.positiveInfinity;
    [SerializeField]
    private float maxYVelocity = 20f;

    //Allow movement to be disabled, for example when player is dead
    private bool movementEnabled = true;

    [Header("Spaceship Visuals")]
    [SerializeField]
    //Amount to "look" ahead when rotating to target direction
    private Vector3 lookAheadDistance = Vector3.zero;


    private void Awake()
    {
        startPosition = transform.position;
        //Intliase Vel and Target Pos
        currentVelocity = Vector3.zero;
        targetPostion = transform.position;
    }

    //Do Movement
    private void Update()
    {
        if (movementEnabled)
        {
            //Check for movement touch
            if (TouchInput.TouchIsValid(TouchDefines.movementTouchID))
            {
                //Get the amount that that touch has moved in the last frame
                Touch movementTouch = Input.GetTouch(TouchDefines.movementTouchID);
                float shipMovement = TouchInput.GetDeltaTouchMovement(movementTouch).y * inputSensitivity;

                //Update target postion based on player input, clamp to our max screen bounds
                targetPostion += new Vector3(0, shipMovement, 0);
                targetPostion = MathUtils.ClampVec3(targetPostion, minPosition, maxPosition);
            }

            //Update postion using dampening so that movement is smooth
            transform.position = Vector3.SmoothDamp(transform.position, targetPostion, ref currentVelocity, movementDampening, maxYVelocity);
        }

        //Look at our target direction with an amount to look ahead (so we don't just look straight up) 
        //and add some randomness to the rotation
        transform.LookAt(targetPostion + lookAheadDistance);
    }

    /// <summary>
    /// Disable movement from player input
    /// </summary>
    private void DisableMovement()
    {
        //Disable movement and stop the player moving to a new
        //position
        movementEnabled = false;
        targetPostion = transform.position;
        currentVelocity = Vector3.zero;
    }

    private void EnableMovement()
    {
        movementEnabled = true;
    }

    /// <summary>
    /// Reset the player postion and velcoity
    /// </summary>
    private void ResetMovement()
    {
        transform.position = startPosition;
        currentVelocity = Vector3.zero;
        targetPostion = transform.position;
        EnableMovement();
    }

    #region Events Sub/UnSub
    private void OnEnable()
    {
        SpaceshipData.PlayerKilled += DisableMovement;
        GameStateManager.LevelStarted += ResetMovement;
        GameStateManager.LevelCompleted += DisableMovement;
        GameStateManager.LevelPaused += DisableMovement;
        GameStateManager.LevelUnPaused += EnableMovement;
        GameStateManager.LevelFailed += DisableMovement;
    }

    private void OnDisable()
    {
        SpaceshipData.PlayerKilled -= DisableMovement;
        GameStateManager.LevelStarted -= ResetMovement;
        GameStateManager.LevelCompleted -= DisableMovement;
        GameStateManager.LevelPaused -= DisableMovement;
        GameStateManager.LevelUnPaused -= EnableMovement;
        GameStateManager.LevelFailed -= DisableMovement;
    }

    #endregion

}
