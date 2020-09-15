using UnityEngine;
using TMPro;
using Lewis.Score;

public class LevelCompleteUI : Menu
{
    [SerializeField]
    private TextMeshProUGUI coinsCollectedText;
    private const string coinsPrefix = "CASH COLLECTED: $";

    private void OnEnable()
    {
        //Update UI Elements
        UpdateLatestScoreText();
    }

    /// <summary>
    /// Update the latest score text with the current
    /// score
    /// </summary>
    private void UpdateLatestScoreText()
    {
        float score = ScoreKeeper.CurrentScore;
        coinsCollectedText.text = coinsPrefix + score.ToString();
    }

}
