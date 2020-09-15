using System.Collections;
using System.Collections.Generic;
using Lewis.Camera;
using UnityEngine;

/// <summary>
/// Class for data needed by the AI Car
/// </summary>
public class AICarData : CarData
{
    [Header("Audio")] 
    [SerializeField] private SFXAudioSource sirenSource;
    [SerializeField] private AudioClip sirenSound;

    //AI Control Element
    private AICarChase carAIChase;

    //Time after disable to destroy after
    private const float despawnTime = 25f; 

    //Events for Car Created/Destroyed so that we can be added/removed fro AI spawner list
    public delegate void AICarSpawnEvent(AICarData aiCar);

    public static event AICarSpawnEvent CarCreated;
    public static event AICarSpawnEvent CarDisabled;


    // Start is called before the first frame update
    private new void Start()
    {
        base.Start();
        CarCreated?.Invoke(this);
        carAIChase = GetComponent<AICarChase>();
        controllingCar = GetComponent<CarPhysics>();

        //Start Siren Sound
        if (sirenSource & sirenSound)
        {
            sirenSource.PlaySFXLoop(sirenSound);
        }
    }


    /// <summary>
    /// Disable the Car
    /// </summary>
    protected override void DisableCar()
    {

        //Call Base Function
        base.DisableCar();

        if (carAIChase)
        {
            carAIChase.enabled = false;
        }

        //Set the inputs of the car to 0 so it stops
        if (controllingCar)
        {
            controllingCar.CarSteerInput = 0f;
            controllingCar.CarAccelerateInput = 0f;
        }


        //Remove as an active car
        CarDisabled?.Invoke(this);

        //Set an coroutine to destroy this object when it is out of view
        Destroy(gameObject , despawnTime);
    }

    private void OnDestroy()
    {
        CarDisabled?.Invoke(this);
    }
}
