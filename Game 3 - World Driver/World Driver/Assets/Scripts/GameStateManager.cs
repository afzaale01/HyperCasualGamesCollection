using System;
using System.Collections;
using System.Collections.Generic;
using Lewis.GameOptions;
using Lewis.Score;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    //Events for Game Start/End
    public delegate void GameStateEvent();
    public static event GameStateEvent LevelStarted;
    public static event GameStateEvent LevelFailed;
    public static event GameStateEvent LevelCompleted;

    //Pausing
    public static event GameStateEvent LevelPaused;
    public static event GameStateEvent LevelUnPaused;

    //Store the player car, so that we can pass this between the menu and game scene
    private static CarInfo playerCarInfo;

    public static CarInfo PlayerSelectedCar
    {
        get { return playerCarInfo; }
        set
        {
            if (value != null)
            {
                playerCarInfo = value;
            }
        }
    }

    /// <summary>
    /// Start the Game Level
    /// </summary>
    public static void StartLevel()
    {
        LevelStarted?.Invoke();
    }

    /// <summary>
    /// Trigger that the level has failed
    /// </summary>
    public void TriggerLevelFailed()
    {
        LevelCompleted?.Invoke();
    }


    public static void PauseGame()
    {
        throw new NotImplementedException();
    }
    public static void ResumeGame()
    {
        throw new NotImplementedException();
    }

    public static void RestartGame()
    {
        //Reload the Scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Add the current score to the leaderboard
    /// </summary>
    private void AddCurrentScoreToLeaderboard()
    {
        float currentScore = ScoreKeeper.CurrentScore;
        string playerName = GameOptionsManager.GetCurrentGameOptions().playerName;
        ScoreLocalLeaderboards.AddScore(playerName, currentScore, SceneManager.GetActiveScene().name);
    }

    #region Event Subs/UnSubs

    private void OnEnable()
    {
        PlayerData.PlayerKilled += TriggerLevelFailed;
        PlayerData.PlayerKilled += AddCurrentScoreToLeaderboard;
    }

    private void OnDisable()
    {
        PlayerData.PlayerKilled -= TriggerLevelFailed;
        PlayerData.PlayerKilled -= AddCurrentScoreToLeaderboard;
    }

    #endregion

}
