using System.Collections;
using System.Collections.Generic;
using Lewis.GameOptions;
using UnityEngine;

/// <summary>
/// Class for playing SFX audio at the correct volume as
/// set by the options
/// </summary>
public class SFXAudioSource : MonoBehaviour
{
    [SerializeField]
    private AudioSource playSource;

    private void Awake()
    {
        //Create the audio soruce for playing SFX
        if (playSource == null)
        {
            playSource = gameObject.AddComponent<AudioSource>();
            playSource.playOnAwake = false;
            playSource.loop = false;
            playSource.priority = 10;
        }

        //Update Volume
        UpdateAudioSourceVolume();
    }

    /// <summary>
    /// Play a SFX on this audio soruce
    /// </summary>
    /// <param name="sfxAudio">Audio clip to play</param>
    public void PlaySFX(AudioClip sfxAudio)
    {
        if (sfxAudio && playSource)
        {
            playSource.loop = false; //Make sure we are not looping
            playSource.clip = sfxAudio;
            playSource.Play();
        }
    }

    /// <summary>
    /// Play a sfx on loop on this audio source
    /// </summary>
    /// <param name="sfxAudio">Audio clip to play</param>
    public void PlaySFXLoop(AudioClip sfxAudio)
    {
        //Play sound on loop
        if (sfxAudio && playSource)
        {
            playSource.loop = true;
            playSource.clip = sfxAudio;
            playSource.Play();
        }
    }

    /// <summary>
    /// Update the volume of this audio source to the user selected
    /// SFX volume
    /// </summary>
    private void UpdateAudioSourceVolume()
    {
        //Set the audio soruce volume to the set SFX volume
        if (playSource)
        {
            playSource.volume = GameOptionsManager.GetCurrentGameOptions().sfxVolume;
        }
    }


    #region Event Subs/Unsubs

    private void OnEnable()
    {
        //Update the audio soruce volume when we get new optoions
        GameOptionsManager.GameOptionsUpdated += UpdateAudioSourceVolume;
    }

    private void OnDisable()
    {
        GameOptionsManager.GameOptionsUpdated -= UpdateAudioSourceVolume;
    }

    #endregion
}
