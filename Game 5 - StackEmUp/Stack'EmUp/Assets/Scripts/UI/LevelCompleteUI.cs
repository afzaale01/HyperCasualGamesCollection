using UnityEngine;
using TMPro;
using Lewis.Score;

public class LevelCompleteUI : Menu
{
    [SerializeField]
    private TextMeshProUGUI coinsCollectedText;
    private const string coinsPrefix = "Objects Stacked: ";

    [Header("Audio")]
    [SerializeField] private SFXAudioSource loseAudioSource;
    [SerializeField] private AudioClip loseSound;


    private void Start()
    {
        //Update UI Elements
        UpdateLatestScoreText();

        //Play Lose Sound when the player fails
        if (loseAudioSource && loseSound)
        {
            loseAudioSource.PlaySFX(loseSound);
        }
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
