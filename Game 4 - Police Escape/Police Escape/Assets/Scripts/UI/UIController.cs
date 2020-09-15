using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class handles transisions between different in game uis
/// </summary>
public class UIController : MonoBehaviour
{
    //UI Objects
    [SerializeField]
    private GameObject gameRunningUI, gamePausedUI, levelCompleteUI;

    private GameObject activeUI;

    private void ShowUI(GameObject uiObject)
    {
        //Null Check UI exists
        if (!uiObject)
        {
            return;
        }

        //Hide existing UI
        if(activeUI != null)
        {
            activeUI.SetActive(false);
        }

        //Show New UI
        uiObject.SetActive(true);
        activeUI = uiObject;
    }

    /// <summary>
    /// Show the game runnig UI (and hide all others)
    /// </summary>
    private void ShowGameRunningUI()
    {
        ShowUI(gameRunningUI);
    }

    /// <summary>
    /// Show the pause UI (and hide all others)
    /// </summary>
    private void ShowPauseUI()
    {
        ShowUI(gamePausedUI);
    }

    /// <summary>
    /// Show the level complete UI (and hide all others)
    /// </summary>
    private void ShowLevelCompleteUI()
    {
        ShowUI(levelCompleteUI);
    }

    #region Event Subscribes/Unsubscribes
    private void OnEnable()
    {
        GameStateManager.LevelStarted += ShowGameRunningUI;
        GameStateManager.LevelPaused += ShowPauseUI;
        GameStateManager.LevelUnPaused += ShowGameRunningUI;
        GameStateManager.LevelCompleted += ShowLevelCompleteUI;
    }

    private void OnDisable()
    {
        GameStateManager.LevelStarted -= ShowGameRunningUI;
        GameStateManager.LevelPaused -= ShowPauseUI;
        GameStateManager.LevelUnPaused -= ShowGameRunningUI;
        GameStateManager.LevelCompleted -= ShowLevelCompleteUI;
    }
    #endregion

}
