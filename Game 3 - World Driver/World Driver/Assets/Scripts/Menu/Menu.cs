using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class that is used for functions used by all game menus
/// </summary>
public class Menu : MonoBehaviour
{
    private const string mainMenuSceneName = "SCN_MenuMain";

    /// <summary>
    /// Loads the main game menu
    /// </summary>
    public void LoadMainMenu()
    {
        LoadMenuScene(mainMenuSceneName);
    }

    /// <summary>
    /// Loads a specified menu scene
    /// </summary>
    /// <param name="sceneName"></param>
    public void LoadMenuScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
