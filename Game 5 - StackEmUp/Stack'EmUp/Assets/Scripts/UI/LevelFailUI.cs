using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for Failed Level UI
/// </summary>
public class LevelFailUI : MonoBehaviour
{
    [Header("Audio")] 
    [SerializeField] private SFXAudioSource loseAudioSource;
    [SerializeField] private AudioClip loseSound;
    
    private void Start()
    {
        //Play Lose Sound when the player fails
        if (loseAudioSource && loseSound)
        {
            loseAudioSource.PlaySFX(loseSound);
        }
    }
}
