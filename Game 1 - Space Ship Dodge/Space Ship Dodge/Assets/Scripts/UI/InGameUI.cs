using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lewis.Score;
using TMPro;

public class InGameUI : Menu
{
    [SerializeField]
    private TextMeshProUGUI scoreText;

    /// <summary>
    /// Update the Score Text
    /// </summary>
    public void UpdateScoreText(float score)
    {
        scoreText.text = score.ToString();
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
