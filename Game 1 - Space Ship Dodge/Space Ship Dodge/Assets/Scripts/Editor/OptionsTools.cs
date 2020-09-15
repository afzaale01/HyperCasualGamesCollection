using System.Collections;
using System.Collections.Generic;
using System.IO;
using Lewis.GameOptions;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor Script to Reset Options  
/// </summary>
public class OptionsTools : MonoBehaviour
{
    /// <summary>
    /// Resets the options file
    /// </summary>
    [MenuItem("Game IO/Options/Reset Options")]
    public static void DeleteLeaderboardFile()
    {
        string optionsFile = GameOptionsManager.GetOptionsSaveLocation();
        if (File.Exists(optionsFile))
        {
            //Delete existing options file
            File.Delete(optionsFile);
            Debug.Log("[Success] Reset all options from file: " + optionsFile);

            //Save a new set of default options
            GameOptionsManager.SaveGameOptions(new GameOptionsInfo());
        }
        else
        {
            Debug.LogWarning("[Failed] Could not delete options file. Does not exist or is in use: " + optionsFile);
        }
    }
}
