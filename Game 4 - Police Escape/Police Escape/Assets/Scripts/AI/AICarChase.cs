using System;
using System.Collections;
using System.Numerics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// Class which generates a path that a car can follow to a target using
/// Unity's Nav Mesh and NavMeshAgent
/// </summary>
[RequireComponent(typeof(CarPhysics))]
public class AICarChase : MonoBehaviour
{
    //Object that we are chasing
    [SerializeField] private GameObject target;

    //AI agent that will do our pathfinding, this cannot be on the object the car physics is 
    //on as it takes over physics control
    [Header("AI")]
    [SerializeField]
    private NavMeshAgent aiAgent;
    //The latest path that we have generated
    private NavMeshPath latestPath;

    //Car Physics Component that we wil drive
    private CarPhysics controllingCar;

    //Minimum distance that we should be away from a point in the path
    //to say we have hit it
    private const float minPointHitDist = 1f;

    //Values for slowing down close to corners
    [Header("Acceleration")]
    [SerializeField] private float normalAcceleration = 1f; //Acceleration when we are not approaching a corner


    //Timer for if the car is stuck
    [Header("Stuck Settings")]
    [SerializeField] private float carStuckVelocity = 0.5f; //Min velocity to count as stuck
    [SerializeField] private float carStuckTimeLimit = 2f; //How long we are allowed to be stuck for
    [SerializeField] private float carReverseTime = 3f; //Time we should reverse for after being stuck
    private float carStuckTimer; //Time we have been stuck for

    //Gear (forward or reverse for our car)
    private enum Gear
    {
        Reverse = -1,
        Forward = 1
    }

    private Gear currentGear;

    //todo remove
    private Vector3 nextPos;

    private void Start()
    {
        //NullCheck our nav agent
        if (aiAgent == null)
        {
            Debug.LogWarning("AI Car does not have a nav mesh agent assigned");
        }
        //Initalise our path to be an empty path
        latestPath = new NavMeshPath();

        //Get the Car we are controlling, this is a required component so we will have it,
        //thus no null check
        controllingCar = GetComponent<CarPhysics>();

        //Set us to move forwards
        currentGear = Gear.Forward;

        //Set our car stuck timer to 0 
        carStuckTimer = 0f;

        //If we do not have a target then try and find the player
        if (target == null)
        {
            target = FindObjectOfType<PlayerMovement>().gameObject;
        }
    }

    private void FixedUpdate()
    {
        //Set out Ai agents positio so we always have an up to date path
        SetAgentPosition();

        //Update the path every physics update
        latestPath = GeneratePath(target.transform.position);

        if (latestPath.corners == null || latestPath.corners.Length == 0)
        {
            return;
        }

        //Get the next position that we can get to in the path
        nextPos = GetNextPosInPath(latestPath);

        //If the car is stuck then set it in reverse for a 
        //length of time to try and get out
        if (IsCarStuck())
        {
            if (currentGear != Gear.Reverse)
            {
                carStuckTimer += Time.deltaTime;
                if (carStuckTimer > carStuckTimeLimit)
                {
                    StartCoroutine(SetGearForTime(Gear.Reverse, carReverseTime));
                    carStuckTimer = 0f;
                }
            }
        }


        //Get the angle to the next position
        float angleToNextPos = Vector3.SignedAngle(transform.forward, nextPos - transform.position, transform.up);
        //Get the acceleration that we need
        float accelerationInput = normalAcceleration * (int)currentGear;

        //Relay Inputs to Car
        controllingCar.CarSteerAngle = angleToNextPos * (int)currentGear;
        controllingCar.CarAccelerateInput = accelerationInput;
    }

    /// <summary>
    /// Generates a navmesh path to a target object
    /// </summary>
    private NavMeshPath GeneratePath(Vector3 targetPos)
    {
        //Generate a path using unitys navmesh system
        NavMeshPath generatedPath = new NavMeshPath(); ;
        aiAgent.CalculatePath(targetPos, generatedPath);
        return generatedPath;
    }

    /// <summary>
    /// Gets the next position in the path, ignoring positions that
    /// we are already at
    /// </summary>
    /// <returns>The next viable position in the path, or a vector 3 of Positive Infinity if no point is found</returns>
    private Vector3 GetNextPosInPath(NavMeshPath path)
    {
        /*
         * We need to be able to dismiss points that the car has reached but not the
         * nav mesh agent so we look around a car and check if the next point in the path is
         * within a given radius. If it is then we choose the second to next point in the path
         * so that our movement is smooth
         */

        if (path != null)
        {
            //Loop all of the positions that we have and check if there are any 
            //that are over the distance we need, if there is one return it
            foreach (Vector3 pathPos in path.corners)
            {
                if (Vector3.Distance(transform.position, pathPos) > minPointHitDist)
                {
                    return pathPos;
                }
            }
        }

        //No points found or path invalid, return positive infinity vector
        return Vector3.positiveInfinity;
    }

    /// <summary>
    /// Checks if the car is stuck (i.e it has not moved for the last x seconds and has a
    /// valid path)
    /// </summary>
    /// <returns></returns>
    private bool IsCarStuck()
    {
        //Check we have a valid path, if we don't have one
        //then don't check if we aren't moving because we shouldn't be
        if (latestPath != null)
        {
            //Check if we are stuck - i.e no velocity
            Rigidbody carRb = controllingCar.GetComponent<Rigidbody>();
            if (carRb != null)
            {
                if (carRb.velocity.magnitude < carStuckVelocity)
                {
                    return true;
                }
            }
        }

        //Either no path or velocity is > than the stuck velocity
        return false;
    }

    /// <summary>
    /// Set out AI agent position to the closest position on
    /// the nav mesh
    /// </summary>
    void SetAgentPosition()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position,
            out hit, 1.0f,
            NavMesh.AllAreas))
        {
            aiAgent.transform.position = hit.position;
        }
    }


    /// <summary>
    /// Coroutine that puts the car in a given gear for a number
    /// of seconds
    /// </summary>
    /// <param name="setGear">Gear to set</param>
    /// <param name="time">Time to set for</param>
    IEnumerator SetGearForTime(Gear setGear, float time)
    {
        Gear originalGear = currentGear;
        currentGear = setGear;
        yield return new WaitForSeconds(time);
        currentGear = originalGear;
    }

    //Debug Drawing Our Path
    private void OnDrawGizmos()
    {
        if (enabled)
        {
            //Debug Drawing
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(nextPos, 0.25f);

            Gizmos.color = Color.blue;
            if (latestPath != null)
            {
                for (int i = 0; i < latestPath.corners.Length; i++)
                {
                    if (i - 1 >= 0)
                    {
                        Gizmos.DrawLine(latestPath.corners[i], latestPath.corners[i - 1]);
                    }
                }
            }

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (transform.forward * 10f));
        }
    }

}