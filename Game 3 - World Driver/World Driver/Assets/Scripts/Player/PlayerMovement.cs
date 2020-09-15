using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lewis.TouchExtensions;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    //Properties for the characteristics of the car
    private float moveSpeed = 10f;
    private float rotationAmount = 30f;
    private const float rotationSpeed = 40f;

    //Rigidbody of this object
    private Rigidbody objectRB;

    //Direction that the player must swipe/drag in to move the player
    //either X or Y. X is best for portrait and Y is best for landscape
    private const TouchInput.SwipeAxis expectedSwipeAxis = TouchInput.SwipeAxis.X;

    [Header("Audio")] 
    [SerializeField] private SFXAudioSource engineAudioSource;
    [SerializeField] private AudioClip engineSound;

    // Start is called before the first frame update
    private void Start()
    {
        objectRB = GetComponent<Rigidbody>();

        if (GameStateManager.PlayerSelectedCar)
        {
            //Get multipliers from the game state manager and apply those to our default values
            moveSpeed *= GameStateManager.PlayerSelectedCar.speedMutiplier;
            rotationAmount *= GameStateManager.PlayerSelectedCar.handilingMutiplier;
        }

        //Start playing car audio
        if (engineAudioSource && engineSound)
        {
            engineAudioSource.PlaySFXLoop(engineSound);
        }

    }

    private void FixedUpdate()
    {
        //Get the amount of input rotation
        float inputRotation = 0f;
        if (TouchInput.TouchIsValid(TouchDefines.movementTouchID))
        {
            //Get the input touch, now we know its valid, then get get the amount
            //that touch has moved in the direction that we use to rotate the car
            //then mutiply it by the rotation amount so it does something significant to 
            //the car
            Touch inputTouch = Input.GetTouch(TouchDefines.movementTouchID);
            inputRotation = TouchInput.GetTouchMovementInDirection(inputTouch,expectedSwipeAxis) * rotationAmount;
        }

        if (objectRB != null)
        {
            //Move the player forward
            objectRB.MovePosition(objectRB.position + transform.forward * (moveSpeed * Time.fixedDeltaTime));

            //Get the current rotation
            Quaternion currentRotation = objectRB.rotation;

            //Get the amount of rotation the user has inputted and apply it in the y direction
            Vector3 yRotation = Vector3.up * (inputRotation * rotationSpeed * Time.fixedDeltaTime);
            //Convert this rotation to a Quaternion
            Quaternion deltaRotation = Quaternion.Euler(yRotation);
            //Work out our target rotation
            Quaternion targetRotation = currentRotation * deltaRotation;
            objectRB.MoveRotation(Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime));
        }
    }
}
