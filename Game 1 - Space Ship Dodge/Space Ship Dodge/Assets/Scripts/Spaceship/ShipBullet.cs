using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for dealing with Bullets Shot from a Ship
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ShipBullet : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed = 20f;

    [SerializeField]
    private float bulletLifetime = 20f;

    //Tag for objects that can be destoryed
    private const string destructableTag = "Destructable";

    //Object that created this bullet
    private GameObject creator = null;
    public GameObject BulletCreator
    {
        get { return creator; }
        set { 
            //Only allow creator to be set once
            if (creator == null)
            {
                creator = value;
            }
        }
    }

    private void Start()
    {
        //Start by setting speed
        GetComponent<Rigidbody>().velocity = moveSpeed * transform.forward;

        //Start Co-Routine to destroy after life time
        StartCoroutine(DestoryAfterLifetime(bulletLifetime));
    }

    /// <summary>
    /// Destory the object after a given life time
    /// </summary>
    /// <param name="time">Time to destroy after</param>
    private IEnumerator DestoryAfterLifetime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    /// <summary>
    /// Processes bullet collision based on the object
    /// it hit
    /// </summary>
    private void ProccessBulletCollision(GameObject other)
    {
        //Only process collision if we are not collding with the creator
        //(i.e our ship)
        if (other != creator)
        {
            //If we hit a destructable object - destory it,
            //then destroy the bullet
            ShootableWall collidedWall = other.GetComponent<ShootableWall>();
            if (collidedWall)
            {
                collidedWall.ExplodeWall();
            }

            //Destory the bullet regardless of what we have hit
            Destroy(gameObject);
        }
    }

    //Destroy when we collide with something
    private void OnCollisionEnter(Collision other)
    {
        ProccessBulletCollision(other.gameObject);
    }

    //Destroy when we collide with something
    private void OnTriggerEnter(Collider other)
    {
        ProccessBulletCollision(other.gameObject);
    }
}
