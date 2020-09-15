using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Lewis.Score;
using Lewis.GameOptions;

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

    private void Start()
    {
        TriggerGameStart();
    }

    public static void PauseGame()
    {
        //Fire event to load game
        //(i.e reset level offset)
        LevelPaused?.Invoke();
    }

    public static void ResumeGame()
    {
        //Fire event to load game
        //(i.e reset level offset)
        LevelUnPaused?.Invoke();
    }


    public static void RestartGame()
    {
        //Reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Triggers actions to start a new game
    /// </summary>
    private void TriggerGameStart()
    {
        //Fire event to load game
        //(i.e reset level offset)
        LevelStarted?.Invoke();
    }

    /// <summary>
    /// Triggers actions to end the current game with a fail state
    /// </summary>
    private void TriggerLevelFailed()
    {
        //Fire Event to end game
        LevelFailed?.Invoke();
    }

    /// <summary>
    /// Trigger actions to end the current game with a success state
    /// </summary>
    private void TriggerLevelComplete()
    {
        LevelCompleted?.Invoke();
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

    #region Event Subscribes/UnSubscribes
    private void OnEnable()
    {
        GameStateManager.LevelStarted += ScoreKeeper.ResetScore;
        GameStateManager.LevelCompleted += AddCurrentScoreToLeaderboard;
        SpaceshipData.PlayerKilled += TriggerLevelFailed;
        LevelEndMarker.LevelEndPassed += TriggerLevelComplete;
    }

    private void OnDisable()
    {
        GameStateManager.LevelStarted -= ScoreKeeper.ResetScore;
        GameStateManager.LevelCompleted -= AddCurrentScoreToLeaderboard;
        SpaceshipData.PlayerKilled -= TriggerLevelFailed;
        LevelEndMarker.LevelEndPassed -= TriggerLevelComplete;
    }

    #endregion

}
