using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lewis.GameOptions;
using TMPro;

public class OptionsMenu : Menu
{

    [SerializeField]
    private Slider musicVolumeSlider, sfxVolumeSlider;

    [SerializeField]
    private TMP_InputField nameInput;

    // Start is called before the first frame update
    void Start()
    {
        //Load in exsiting options
        LoadOptionsInToUI();
    }

    /// <summary>
    /// Apply the game options currently held by ui elements
    /// </summary>
    public void ApplyGameOptions()
    {
        //Convert UI Values in to GameOptions class and pass to save to file
        GameOptionsInfo pendingOptions = new GameOptionsInfo();

        pendingOptions.musicVolume = musicVolumeSlider.value;
        pendingOptions.sfxVolume = sfxVolumeSlider.value;
        pendingOptions.playerName = nameInput.text;

        GameOptionsManager.SaveGameOptions(pendingOptions);
    }

    /// <summary>
    /// Load the existing game options in to options fields
    /// </summary>
    private void LoadOptionsInToUI()
    {
        //Load Options from Manager
        GameOptionsInfo loadedOptions = GameOptionsManager.GetCurrentGameOptions();

        //Load values in to ui elements
        musicVolumeSlider.value = loadedOptions.musicVolume;
        sfxVolumeSlider.value = loadedOptions.sfxVolume;
        nameInput.text = loadedOptions.playerName;
    }


}
