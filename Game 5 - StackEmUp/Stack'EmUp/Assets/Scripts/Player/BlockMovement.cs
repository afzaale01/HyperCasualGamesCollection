using System;
using System.Collections;
using System.Collections.Generic;
using Lewis.TouchExtensions;
using UnityEditor;
using UnityEngine;
using MathUtils = Lewis.MathUtils.MathUtils;

public class BlockMovement : MonoBehaviour
{
    //Target Position and Velocity
    private Vector3 currentVelocity;
    private Vector3 targetPosition; //Target Postion to interpolate to
    private Vector3 startPosition;

    [Header("Input")]
    //Minimum and maximum amount to for the phone to rotate before we move the 
    //platform
    [SerializeField] private float minMoveAngleThreshold = 10f;
    [SerializeField] private float maxMoveAngleThreshold = 90f;
    //Mutiplier for the rotation angle to the amount that the 
    //platform moves
    [SerializeField] private float rotationMutiplier = 0f;
    //This is the angle that the phone reports when straight up
    private const float angleOffset = 90f;

    [Header("Movement")]
    //Max Min positions in the move axis
    [SerializeField] private Vector3 minPosition;
    [SerializeField] private Vector3 maxPosition;
    [SerializeField] private float movementDampening = 0.33f;
    [SerializeField] private float inputSensitivity = 1.0f;
    [SerializeField] private float maxVelocity = 20f;

    //Mask for the direction that we should move the plaform in
    [SerializeField] private Vector3 platformMoveMask;

    //Rigidbody for the block
    private Rigidbody blockRB;

    //If we are allowing movement (i.e not paused)
    private bool allowMovement;

    // Start is called before the first frame update
    void Start()
    {
        blockRB = GetComponent<Rigidbody>();
        allowMovement = true;

        //Intliase Vel and Target Pos
        currentVelocity = Vector3.zero;
        targetPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (allowMovement)
        {
            //Check for movement touch
            if (TouchInput.TouchIsValid(TouchDefines.movementTouchID))
            {
                //Get the amount that that touch has moved in the last frame
                Touch movementTouch = Input.GetTouch(TouchDefines.movementTouchID);
                float platformMovement = TouchInput.GetDeltaTouchMovement(movementTouch).x * inputSensitivity;

                //Update target postion based on player input, clamp to our max screen bounds
                targetPosition += new Vector3(0f, 0f, platformMovement);
                targetPosition = MathUtils.ClampVec3(targetPosition, minPosition, maxPosition);
            }

            //Update postion using dampening so that movement is smooth
            Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, movementDampening, maxVelocity);
            blockRB.velocity = currentVelocity;
        }
    }

    /// <summary>
    /// Allow the player to move
    /// </summary>
    private void StartMovement()
    {
        allowMovement = true;
    }

    /// <summary>
    /// Stop the player being able to move
    /// </summary>
    private void StopMovement()
    {
        allowMovement = false;
    }

    #region Event Subs/UnSubs

    private void OnEnable()
    {
        //Sleep the RB when we pause and unsleep it when 
        //we unpause
        if (blockRB)
        {
            GameStateManager.LevelFailed += blockRB.Sleep;
            GameStateManager.LevelCompleted += blockRB.Sleep;
            GameStateManager.LevelPaused += blockRB.Sleep;
            GameStateManager.LevelUnPaused += blockRB.WakeUp;
        }

        //Stop the player movement when paused
        GameStateManager.LevelPaused += StopMovement;
        GameStateManager.LevelUnPaused += StartMovement;
        GameStateManager.LevelFailed += StopMovement;
        GameStateManager.LevelCompleted += StopMovement;
    }
    private void OnDisable()
    {   
        //Sleep the RB when we pause and unsleep it when 
        //we unpause
        if (blockRB)
        {
            GameStateManager.LevelFailed -= blockRB.Sleep;
            GameStateManager.LevelCompleted -= blockRB.Sleep;
            GameStateManager.LevelPaused -= blockRB.Sleep;
            GameStateManager.LevelUnPaused -= blockRB.WakeUp;
        }

        //Stop the player movement when paused
        GameStateManager.LevelPaused -= StopMovement;
        GameStateManager.LevelUnPaused -= StartMovement;
        GameStateManager.LevelFailed -= StopMovement;
        GameStateManager.LevelCompleted -= StopMovement;
    }

    #endregion
}
