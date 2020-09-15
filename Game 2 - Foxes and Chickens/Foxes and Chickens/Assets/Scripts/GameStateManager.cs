using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    //Inistance of the game state manager
    private static GameStateManager instance;

    //Events for Starts of turns
    public delegate void GameTurnEvent(GameTurn newTurn);
    public static event GameTurnEvent GameTurnStarted;

    //Events for Game Start/End
    public delegate void GameStateEvent();
    public static event GameStateEvent LevelStarted;
    public static event GameStateEvent LevelFailed;
    public static event GameStateEvent LevelCompleted;

    //Events for Pause/UnPause
    public static event GameStateEvent LevelPaused;
    public static event GameStateEvent LevelUnPaused;

    //Store the max amount that a entity can move per turn
    public static readonly float tileMoveAmount = 1f;

    //Enum for game state
    public enum GameTurn
    {
        TURN_PLAYER,
        TURN_AI,
        TURN_NONE,

        TURN_COUNT
    }

    public static GameTurn CurrentTurn { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        //Check for an existing instance and destory
        //if one exists
        if(instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        //Call Level Started Event
        LevelStarted?.Invoke();

        //Start the game with the player's turn
        CurrentTurn = GameTurn.TURN_PLAYER;

        //Call event for new turn started
        GameTurnStarted?.Invoke(CurrentTurn);

    }

    /// <summary>
    /// Switches the game state from either the player -> AI or
    /// AI -> Player
    /// </summary>
    /// <param name="turnEnding">The turn that has just ended</param>
    private void SwitchGameTurn(GameTurn turnEnding)
    {

        //Switch turn only if the current turn has ended
        switch (turnEnding)
        {
            case GameTurn.TURN_AI when CurrentTurn == GameTurn.TURN_AI:
                CurrentTurn = GameTurn.TURN_PLAYER;
                break;
            case GameTurn.TURN_PLAYER when CurrentTurn == GameTurn.TURN_PLAYER:
                CurrentTurn = GameTurn.TURN_AI;
                break;
        }

        //Call event for new turn started
        GameTurnStarted?.Invoke(CurrentTurn);
    }

    public static void PauseGame()
    {
        //Fire event to pause game
        LevelPaused?.Invoke();
    }

    public static void ResumeGame()
    {
        //Fire event to resume game
        LevelUnPaused?.Invoke();
    }


    public static void RestartGame()
    {
        //Fire event to load game
        //(i.e reset level offset)
        LevelStarted?.Invoke();
        LevelUnPaused?.Invoke();

        CurrentTurn = GameTurn.TURN_PLAYER;
    }

    /// <summary>
    /// Triggers the Game Over State
    /// </summary>
    private void TriggerLevelFailed()
    {
        //Set it to be nobody's turn
        CurrentTurn = GameTurn.TURN_NONE;

        //Send Level Failed Event
        LevelFailed?.Invoke();
    }

    /// <summary>
    /// Triggers the Game Over State
    /// </summary>
    private void TriggerLevelComplete()
    {
        //Set it to be nobody's turn
        CurrentTurn = GameTurn.TURN_NONE;

        //Send Level Failed Event
        LevelCompleted?.Invoke();
    }

    #region Event Subs/Unsubs
    private void OnEnable()
    {
        //Game Turns
        Character.EndTurn += SwitchGameTurn;

        PlayerData.PlayerKilled += TriggerLevelFailed;
        EndTile.EndTileReached += TriggerLevelComplete;
    }

    private void OnDisable()
    {
        //Game Turns
        Character.EndTurn -= SwitchGameTurn;

        PlayerData.PlayerKilled -= TriggerLevelFailed;
        EndTile.EndTileReached -= TriggerLevelComplete;
    }
    #endregion
}
