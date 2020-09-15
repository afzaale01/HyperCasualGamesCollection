using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lewis.GameOptions;
using Lewis.TouchExtensions;
using TMPro;

public class OptionsMenu : Menu
{
    [Header("Options")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [SerializeField]
    private TMP_InputField nameInput;

    [Header("Calibration Script")] 
    [SerializeField] private PhoneCalibration calibrationValues;

    [Header("Calibrate UI Toggles")]
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private GameObject calibrateUI;

    // Start is called before the first frame update
    void Start()
    {
        //Load in exsiting options
        LoadOptionsInToUI();
    }

    /// <summary>
    /// Sets if the calibrate UI is shown or the general options UI
    /// </summary>
    /// <param name="showUI">If the UI should be shown or not</param>
    public void ShowCalibrateUI(bool showUI)
    {
        calibrateUI.gameObject.SetActive(showUI);
        optionsUI.gameObject.SetActive(!showUI);
    }

    /// <summary>
    /// Apply the game options currently held by ui elements
    /// Called by button press in inspector
    /// </summary>
    public void ApplyGameOptions()
    {
        //Convert UI Values in to GameOptions class and pass to save to file
        GameOptionsInfo pendingOptions = new GameOptionsInfo
        {
            musicVolume = musicVolumeSlider.value,
            sfxVolume = sfxVolumeSlider.value,
            playerName = nameInput.text,
            phoneRotOffset = calibrationValues.lastCalibrationValues.eulerAngles
        };


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
