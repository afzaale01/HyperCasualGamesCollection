using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Renderer))]
public class CoinPickup : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private SFXAudioSource soundSource;
    [SerializeField] private AudioClip pickupSound;

    [Header("Model")] 
    [SerializeField] private GameObject pickupModel;
    
    //Value of coin
    [SerializeField]
    private float coinValue = 1;
    public float Value { get { return coinValue; } }

    /// <summary>
    /// Register the coin as picked up
    /// </summary>
    public void RegisterPickup()
    {
        //Play sound
        if (soundSource && pickupSound)
        {
            soundSource.PlaySFX(pickupSound);
        }

        //Hide and disable collisions
        pickupModel.SetActive(false);
        GetComponent<Collider>().enabled = false;

        //Destory object after sound has played
        Destroy(gameObject, pickupSound.length);
    }
}
