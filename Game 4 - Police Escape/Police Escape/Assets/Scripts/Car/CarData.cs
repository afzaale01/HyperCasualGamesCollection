using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for storing car data shared between AI and Player
/// </summary>
public abstract class CarData : MonoBehaviour
{
    //Health
    [SerializeField] private float startHealth = 150f;
    private const float maxHealth = 150f;
    public static float MaxHealth
    {
        get { return maxHealth; }
    }
    private float health;

    //Damage Modifier
    [SerializeField] protected float hitDamageModifier = 1f;

    [Header("Death Particles")]
    //Smoke Particles that exist already in the car prefab
    [SerializeField] protected ParticleSystem smokeParticleSystem;

    //Car Physics Component that we wil drive
    protected CarPhysics controllingCar;


    //Enum for state
    protected enum CarState
    {
        Enabled,
        Disabled
    }

    protected CarState currentState;

    //Property for the health of the car
    protected float CarHealth
    {
        get { return health; }
        set { health = Mathf.Clamp(value, 0f, maxHealth); }
    }

    protected void Start()
    {
        //Init Health
        health = startHealth;
        //Set car enabled
        currentState = CarState.Enabled;

        //Make sure the death particles are disabled
        smokeParticleSystem.gameObject.SetActive(false);

        //Force car update incase we spawn dead
        UpdateCarState();
    }


    /// <summary>
    /// Updates the state of the car based
    /// on it's health
    /// </summary>
    protected virtual void UpdateCarState()
    {
        //If our health is below 0 disable the car
        if (CarHealth <= 0)
        {
            DisableCar();
        }
    }

    /// <summary>
    /// Disable the car
    /// </summary>
    protected virtual void DisableCar()
    {
        //Set state
        currentState = CarState.Disabled;

        //Activate the particle system
        smokeParticleSystem.gameObject.SetActive(true);
    }

    //When we collide update the car state
    private void OnCollisionEnter(Collision other)
    {
        //When we collide with something cause us some damage
        float hitSpeed = other.relativeVelocity.magnitude;
        CarHealth -= (hitSpeed * hitDamageModifier);

        //Update the car state to check if we need to be destroyed
        UpdateCarState();
    }
}
