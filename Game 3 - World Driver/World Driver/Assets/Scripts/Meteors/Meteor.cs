using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Meteor : GravityAffectedObject
{
    [Header("Meteor Settings")]
    [SerializeField] 
    private GameObject craterPrefab;

    [Header("Explosion")] 
    [SerializeField] private GameObject explosionPrefab;

    [Header("Meteor Models")] [SerializeField]
    private GameObject[] models;

    private new void Awake()
    {
        base.Awake();

        //Choose a random model to spawn
        SelectRandomModel();
    }

    /// <summary>
    /// Selects a random model for the meteor
    /// </summary>
    private void SelectRandomModel()
    {
        if (models.Length > 0)
        {
            //Choose a random idex of the model
            int modelIndex = Random.Range(0, models.Length);

            //Apply Random Model
            GameObject model = Instantiate(models[modelIndex], gameObject.transform, true);
            model.transform.position = Vector3.zero;
        }
    }


    private void OnCollisionEnter(Collision other)
    {
        //Check if we are colliding with the planet
        if (other.gameObject.GetComponent<PlanetGravity>())
        {
            //Get the first point of contact with the planet
            Vector3 contactPoint = other.contacts[0].point;

            //Do Explosion
            if (explosionPrefab)
            {
                GameObject explosion = Instantiate(explosionPrefab);
                explosion.transform.position = contactPoint;
            }

            if (craterPrefab != null)
            {
                //Create a crater at the contact point
                GameObject crater = Instantiate(craterPrefab);
                crater.transform.position = contactPoint;
            }

            //Destroy the meteor object
            Destroy(gameObject);
        }
    }
}
