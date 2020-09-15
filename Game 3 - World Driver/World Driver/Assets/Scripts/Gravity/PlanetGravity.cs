using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lewis.MathUtils;

/// <summary>
/// Class for planets that have gravity and attract objects
/// </summary>
[RequireComponent(typeof(PlanetData))]
[RequireComponent(typeof(SphereCollider))]
public class PlanetGravity : MonoBehaviour
{
    //Var to store the sphere collider of the 
    //use to calculate the size of the planet
    private SphereCollider planetCollider;

    //Link to the planet data component of the planet
    private PlanetData planetData;

    [SerializeField]
    private float gravityForce = 9.81f;
    
    private void Awake()
    {
        planetCollider = GetComponent<SphereCollider>();
        planetData = GetComponent<PlanetData>();
    }

    /// <summary>
    /// Get the attraction force between another body and this
    /// </summary>
    /// <param name="body">Body to generate attraction force for</param>
    /// <returns></returns>
    public Vector3 GetAttractionForce(Rigidbody body)
    {
        //Get the up direction for gravity
        Vector3 gravityUp = (body.position - transform.position).normalized;
        return (gravityUp * gravityForce);
    }

    /// <summary>
    /// Get the position to place an object on the planets surface
    /// at it's nearest point
    /// </summary>
    /// <param name="body">Body to generate position for</param>
    /// <returns></returns>
    public Vector3 GetPlaceOnSurfacePos(Rigidbody body)
    {
        //Get the up direction fro gravity
        Vector3 gravityUp = (body.position - transform.position).normalized;

        //Mutiply the gravity up by the collider radius and local scale to get the point
        //on the surface of the planet
        return gravityUp * (planetCollider.radius * planetData.PlanetSize);
    }

    /// <summary>
    /// Gets the rotation amount that the player needs to be 
    /// facing so that it is rotated in the same direction
    /// as the planet
    /// </summary>
    /// <returns></returns>
    public Quaternion GetPlanetSurfaceRotation(Rigidbody body)
    {
        //Get the up direction fro gravity
        Vector3 gravityUp = (body.position - transform.position).normalized;
        //Work out the amount that we need to rotate - that is the rotation difference between the
        //player object's up and gravity's up
        Quaternion targetRot = Quaternion.FromToRotation(body.transform.up, gravityUp) * body.rotation;
        return targetRot;
    }
}
