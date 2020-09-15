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

    //Track how long the game has been running for
    //so we can use it for game difficulty
    private static bool gameRunning = false;
    private static float timeSinceGameStart;

    //Difficulty
    //Curve to follow to increase difficulty
    //todo clean this hack
    [SerializeField] private AnimationCurve inputDifficultyCurve;
    private static AnimationCurve gameDifficultyCurve;

    //Property for the current game difficulty, to be used by other systems
    //Samples the difficulty curve at our time since the game started
    public static float GameDifficulty
    {
        get
        {
            return gameDifficultyCurve.Evaluate(timeSinceGameStart);
        }
    }

    private void Start()
    {
        timeSinceGameStart = 0f;

        //Initalise the difficulty curve
        gameDifficultyCurve = inputDifficultyCurve;

        TriggerGameStart();
    }

    private void Update()
    {
        //Increase game running time (if necessary)
        if (gameRunning)
        {
            timeSinceGameStart += Time.deltaTime;
        }
    }

    /// <summary>
    /// Puase the game
    /// </summary>
    public static void PauseGame()
    {
        //Fire event to load game
        //(i.e reset level offset)
        LevelPaused?.Invoke();
        gameRunning = false;
    }

    /// <summary>
    /// Resume the game
    /// </summary>
    public static void ResumeGame()
    {
        //Fire event to load game
        //(i.e reset level offset)
        LevelUnPaused?.Invoke();
        gameRunning = true;
    }

    /// <summary>
    /// Restart the game
    /// </summary>
    public static void RestartGame()
    {
        //Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        gameRunning = false;
    }

    /// <summary>
    /// Triggers actions to start a new game
    /// </summary>
    private void TriggerGameStart()
    {
        //Fire event to load game
        //(i.e reset level offset)
        LevelStarted?.Invoke();
        gameRunning = true;
    }

    /// <summary>
    /// Trigger actions to end the current game with a success state
    /// </summary>
    private void TriggerLevelComplete()
    {
        LevelCompleted?.Invoke();
        gameRunning = false;
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
        PlayerCarData.PlayerKilled += TriggerLevelComplete;
    }

    private void OnDisable()
    {
        GameStateManager.LevelStarted -= ScoreKeeper.ResetScore;
        GameStateManager.LevelCompleted -= AddCurrentScoreToLeaderboard;
        PlayerCarData.PlayerKilled -= TriggerLevelComplete;
    }

    #endregion

}
