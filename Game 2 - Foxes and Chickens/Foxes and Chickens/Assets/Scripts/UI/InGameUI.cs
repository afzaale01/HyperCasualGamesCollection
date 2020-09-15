using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameUI : Menu
{
    [SerializeField]
    private TextMeshProUGUI currentTurnText;

    //Dictonary of strings to display for each turn
    private readonly Dictionary<GameStateManager.GameTurn, string> turnNamePairs = new Dictionary<GameStateManager.GameTurn, string>()
    {
        { GameStateManager.GameTurn.TURN_AI , "FOXES TURN" },
        { GameStateManager.GameTurn.TURN_PLAYER , "YOUR TURN" },
        { GameStateManager.GameTurn.TURN_NONE , "" }
    };

    /// <summary>
    /// Update the Score Text
    /// </summary>
    public void UpdateTurnText(GameStateManager.GameTurn newTurn)
    {
        //Update Turn Text to whatever is in the dictonary
        //for the given turn
        if (turnNamePairs.ContainsKey(newTurn))
        {
            currentTurnText.text = turnNamePairs[newTurn];
        }

    }

    /// <summary>
    /// Trigger the game to be paused
    /// </summary>
    public void TriggerPauseGame()
    {
        GameStateManager.PauseGame();
    }

    #region Event Subs/UnSubs
    private void OnEnable()
    {
        GameStateManager.GameTurnStarted += UpdateTurnText;
    }
    private void OnDisable()
    {
        GameStateManager.GameTurnStarted -= UpdateTurnText;
    }
    #endregion

}
