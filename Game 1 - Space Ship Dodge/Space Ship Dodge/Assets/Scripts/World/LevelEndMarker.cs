using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndMarker : MonoBehaviour
{

    private const string playerTag = "Player";

    public delegate void LevelMarkerEvent();
    public static event LevelMarkerEvent LevelEndPassed;

    private void OnTriggerEnter(Collider other)
    {
        //Check that the player passed the end of the level
        //then fire event to tell other classes
        if (other.CompareTag(playerTag))
        {
            LevelEndPassed?.Invoke();
        }
    }
}
