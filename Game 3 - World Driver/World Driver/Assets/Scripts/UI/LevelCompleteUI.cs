using UnityEngine;
using TMPro;
using Lewis.Score;
using UnityEngine.Serialization;

public class LevelCompleteUI : Menu
{
    [Header("Audio")] 
    [SerializeField] private SFXAudioSource loseAudioSource;
    [SerializeField] private AudioClip loseSound;

    [Header("Text")]
    [FormerlySerializedAs("coinsCollectedText")] [SerializeField]
    private TextMeshProUGUI scoreText;
    private const string scorePrefix = "SCORE: ";

    private void Start()
    {
        //Update UI Elements
        UpdateLatestScoreText();

        //Play Audio
        loseAudioSource.PlaySFX(loseSound);
        
    }

    /// <summary>
    /// Update the latest score text with the current
    /// score
    /// </summary>
    private void UpdateLatestScoreText()
    {
        float score = ScoreKeeper.CurrentScore;
        scoreText.text = scorePrefix + score.ToString();
    }
}
