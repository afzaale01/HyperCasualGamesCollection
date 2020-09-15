using UnityEngine;
using TMPro;
using Lewis.Score;

public class LevelCompleteUI : GameMenu
{
    [Header("Audio")] 
    [SerializeField] private SFXAudioSource winAudioSource;
    [SerializeField] private AudioClip winSound;

    [Header("Text")]
    [SerializeField]
    private TextMeshProUGUI coinsCollectedText;
    private const string coinsPrefix = "Coins Collected: ";

    private void Start()
    {
        //Update UI Elements
        UpdateLatestScoreText();

        //Play Win Sound
        if (winAudioSource && winSound)
        {
            winAudioSource.PlaySFX(winSound);
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
