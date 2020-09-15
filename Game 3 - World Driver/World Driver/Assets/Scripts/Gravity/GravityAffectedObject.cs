using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for an object that is attacted to a planet object
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class GravityAffectedObject : MonoBehaviour
{
    //Planet that this object is attracted to
    [SerializeField]
    protected PlanetGravity planet;

    //If we should always have the object placed on the planets surface
    //by teleporting - rather than applying force
    [SerializeField]
    private bool placeOnSurface = false;

    //Rigidbody of this object
    protected Rigidbody objectRB;

    //Amount to smooth the rotation of the car
    private float rotationSmoothing = 50f;

    // Start is called before the first frame update
    protected void Awake()
    {
        //Find the rigidbody attached to this object
        objectRB = GetComponent<Rigidbody>();

        //If we don't have a planet then find the closet one and assign it
        if(planet == null)
        {
            planet = FindClosestPlanet();
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (objectRB != null)
        {
            //If this object should be locked to the surface then set its position
            //to the neareset surface position, otherwise apply force to attract this
            //object to the surface
            if (placeOnSurface)
            {
                objectRB.MovePosition(planet.GetPlaceOnSurfacePos(objectRB));
            }
            else
            {
                objectRB.AddForce(planet.GetAttractionForce(objectRB));
            }

            //Rotate the rigidbody so that it's rotation matches that of the planets surface
            objectRB.MoveRotation(Quaternion.Slerp(objectRB.rotation, planet.GetPlanetSurfaceRotation(objectRB), Time.deltaTime * rotationSmoothing));
        }
    }

    /// <summary>
    /// Find the closest planet
    /// </summary>
    /// <returns>The closest planet</returns>
    private PlanetGravity FindClosestPlanet()
    {
        //Store the closest planet and distance
        float closestDist = Mathf.Infinity;
        PlanetGravity closestPlanet = null;

        //Get all of the planet scripts
        PlanetGravity[] allPlanets = FindObjectsOfType<PlanetGravity>();
        foreach(PlanetGravity planet in allPlanets)
        {
            //If the distance to this planet is less than the current distnace then
            //set the new closest
            if(Vector3.Distance(planet.gameObject.transform.position, transform.position) < closestDist)
            {
                closestPlanet = planet;
                closestDist = Vector3.Distance(planet.gameObject.transform.position, transform.position);
            }
        }

        //Return the closest planet - this will be null if there are no planets
        return closestPlanet;
    }
}
