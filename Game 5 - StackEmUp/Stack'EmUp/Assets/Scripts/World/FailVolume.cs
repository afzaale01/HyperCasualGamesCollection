using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

/// <summary>
/// Class to detect if a block falls off the platform at thus
/// the game fails
/// </summary>
public class FailVolume : MonoBehaviour
{

    //Event for an object passing through the volume
    public delegate void FailVolEvent();
    public static event FailVolEvent FailVolTriggered;

    private void OnTriggerEnter(Collider other)
    {
        //Trigger Game Failed event
        FailVolTriggered?.Invoke();

        //Destroy so we can't double trigger
        Destroy(gameObject);
    }
}
