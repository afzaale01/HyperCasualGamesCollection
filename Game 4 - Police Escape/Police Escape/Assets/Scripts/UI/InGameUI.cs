using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lewis.Score;
using TMPro;

public class InGameUI : Menu
{
    [SerializeField]
    private TextMeshProUGUI scoreText;

    //Prefix for score
    private const string scorePrefix = "$";

    /// <summary>
    /// Update the Score Text
    /// </summary>
    public void UpdateScoreText(float score)
    {
        //Format the score using Standard numeric format strings - 
        //https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings?redirectedfrom=MSDN#NFormatString
        scoreText.text = scorePrefix + score.ToString("N0");
    }

    /// <summary>
    /// Trigger the game to be paused
    /// </summary>
    public void TriggerPauseGame()
    {
        GameStateManager.PauseGame();
    }

    #region Event Subs/Unsubs
    private void OnEnable()
    {
        ScoreKeeper.UpdateScoreUI += UpdateScoreText;
    }
    private void OnDisable()
    {
        ScoreKeeper.UpdateScoreUI -= UpdateScoreText;
    }
    #endregion  

}
