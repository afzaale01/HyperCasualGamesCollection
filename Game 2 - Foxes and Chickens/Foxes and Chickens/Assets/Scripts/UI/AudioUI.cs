using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI for when the player loses
/// </summary>
public class AudioUI : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private SFXAudioSource audioSource;
    [SerializeField] private AudioClip sound;

    private void Start()
    {
        //Play Lose Sound when the player fails
        if (audioSource && sound)
        {
            audioSource.PlaySFX(sound);
        }
    }
}
