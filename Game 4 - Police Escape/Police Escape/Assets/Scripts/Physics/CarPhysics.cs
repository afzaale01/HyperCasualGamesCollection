using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Class deals with the physics interactions of the car in the world
/// by using Unity's built in Wheel Colliders
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class CarPhysics : MonoBehaviour
{
    [Header("Car Settings")]
    [SerializeField] private float maxSpeed; //in meters per second
    [SerializeField] private Vector3 carCenterOfMass;

    //Array of wheels that this car will drive
    [Header("Wheels")] 
    [SerializeField] private WheelInfo[] carWheels;

    [Header("Wheel Settings")]
    //Car wheel settings these are values that we will feed
    //in to the wheel colliders
    [SerializeField] private float motorPower = 5000f;
    [SerializeField] private float maxSteerAngle = 35f;

    [Tooltip("How much of our grip we keep when taking sharp turns/acceleration")]
    [Range(0.01f, 0.99f)] [SerializeField] private float keepGrip = 0.99f;
    [SerializeField] private float wheelGrip = 5f;

    //Const values used for things that we don't set in the editor and don't
    //effect the car handiling
    //These are values for our slip graph found here https://docs.unity3d.com/Manual/class-WheelCollider.html
    [SerializeField] private WheelCurveValues forwardCurveValues, sidewaysCurveValues;

    //Input forces to drive the car forward/steering
    //Can be updated by player or AI Input
    private float steerInput = 0f;
    private float accelerateInput = 0f;
    //Properties to update these inputs
    //Steer Input is in range -1 to 1 (i.e a wheel position), used by player
    public float CarSteerInput
    {
        set { steerInput = Mathf.Clamp(value, -1f, 1f); }
    }
    //Steer Angle is in range of our -maxSteerAngle to maxSteerAngle, used by AI and just updates steer input
    public float CarSteerAngle
    {
        set { CarSteerInput = value / maxSteerAngle; }
    }
    public float CarAccelerateInput
    {
        set { accelerateInput = Mathf.Clamp(value, -1f, 1f); }
    }

    //Rigidbody of the car
    private Rigidbody carRB;

    private void Awake()
    {
        carRB = GetComponent<Rigidbody>();
        if (carRB == null)
        {
            Debug.LogWarning("CAR DOES NOT HAVE REQUIRED RIGIDBODY ATTACHED!");
        }

        //Set the cars center of mass to the user value
        carRB.centerOfMass = carCenterOfMass;

        //Validate the wheel settings
        ValidateWheelSettings();
    }

    private void FixedUpdate()
    {

        if (carWheels == null)
        {
            return;
        }

        /*On every physics update, update all the wheels*/
        for (int i = 0; i < carWheels.Length; i++)
        {
            WheelInfo currentWheel = carWheels[i];
            float currentSpeed = carRB.velocity.magnitude;

            //If wheels have motor or steering apply forces 
            if (currentWheel.motor)
            {
                //Only accelerate if we are under the max speed
                if (currentSpeed < maxSpeed)
                {
                    currentWheel.collider.motorTorque = accelerateInput * motorPower;
                }
                else
                {
                    currentWheel.collider.motorTorque = 0f;
                }
            }
            if (currentWheel.steer)
            {
                currentWheel.collider.steerAngle = steerInput * maxSteerAngle;
            }

            //Apply rotation to the wheels and update the wheels mesh renderer
            const float secondsInMin = 60f;
            const float degreesInCircle = 360f;
            currentWheel.rotation = currentWheel.collider.rpm / secondsInMin * degreesInCircle * currentSpeed;

            if (currentWheel.meshRenderer != null)
            {
                currentWheel.meshRenderer.transform.localRotation =
                    Quaternion.Euler(0f, currentWheel.collider.steerAngle, currentWheel.rotation);
            }

        }

        //Apply force to rigidbody
        const float forceModifier = -0.1f;
        Transform carTransform = transform;
        carRB.AddForceAtPosition(carTransform.up * (carRB.velocity.magnitude * forceModifier * wheelGrip), carTransform.position + (carTransform.rotation * carCenterOfMass));
    }

    private void OnValidate()
    {
        /*Validate function called when editor values are changed
         here we want to update all of the wheels settings when values
         are changed*/
        ValidateWheelSettings();
    }

    /// <summary>
    /// Validate the wheel settings by 
    /// </summary>
    private void ValidateWheelSettings()
    {

        if (carWheels == null)
        {
            return;
        }

        //Loop through all of the wheels and apply the wheel settings
        for (int i = 0; i < carWheels.Length; i++)
        {
            WheelCollider currentWheelCollider = carWheels[i].collider;
            if (currentWheelCollider == null)
            {
                return;
            }


            //Get the friction for the wheels so that we can edit the values
            WheelFrictionCurve forwardFriction = currentWheelCollider.forwardFriction;
            WheelFrictionCurve sidewaysFriction = currentWheelCollider.sidewaysFriction;

            //Forward Friction Values
            forwardFriction.asymptoteValue = forwardFriction.extremumValue * keepGrip;
            forwardFriction.stiffness = wheelGrip;
            forwardFriction.extremumSlip = forwardCurveValues.extremumSlip;
            forwardFriction.asymptoteSlip = forwardCurveValues.asymptoteSlip;

            //Sideways Friction values
            sidewaysFriction.asymptoteValue = sidewaysFriction.extremumValue * keepGrip;
            sidewaysFriction.stiffness = wheelGrip;
            sidewaysFriction.extremumValue = sidewaysCurveValues.extremumValue;
            sidewaysFriction.extremumSlip = sidewaysCurveValues.extremumSlip;
            sidewaysFriction.asymptoteSlip = sidewaysCurveValues.asymptoteSlip;

            //Apply new values to wheel
            currentWheelCollider.forwardFriction = forwardFriction;
            currentWheelCollider.sidewaysFriction = sidewaysFriction;

        }
    }

    void OnDrawGizmos()
    {
        //Debug Drawing of Center of Mass
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + carCenterOfMass, .1f);

    }


    /// <summary>
    /// Struct to neatly store the const values for our wheels.
    /// These are not calculated at runtime so we can just store them
    /// in a struct for neatness
    /// </summary>
    [System.Serializable]
    public struct WheelCurveValues
    {
        public float extremumValue;
        public float extremumSlip;
        public float asymptoteSlip;
    }

    /// <summary>
    /// Struct to store infomation about wheels
    /// </summary>
    [System.Serializable]
    public struct WheelInfo
    {
        public WheelCollider collider;
        public MeshRenderer meshRenderer;
        public bool steer;
        public bool motor;
        [HideInInspector] public float rotation;
    }

}
