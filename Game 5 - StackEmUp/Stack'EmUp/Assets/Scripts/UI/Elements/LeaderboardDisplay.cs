using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Lewis.Score;
using TMPro;

public class LeaderboardDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI[] leaderboardNames, leaderboardScores;

    private void Start()
    {
        //Update leaderboard when enabled
        UpdateLeaderboardText();
    }

    /// <summary>
    /// Update the leaderboard text elements
    /// </summary>
    private void UpdateLeaderboardText()
    {

        //Early out if we have not text elements to write to
        if (leaderboardNames == null || leaderboardScores == null)
        {
            return;
        }

        //Get leaderboard info and the number of scores we need
        //to fill in
        var leaderboardInfo = ScoreLocalLeaderboards.GetSortedScores(SceneManager.GetActiveScene().name);
        int scoreCount = Mathf.Min(leaderboardNames.Length, leaderboardScores.Length);

        //Update scores text
        for (int i = 0; i < scoreCount; i++)
        {
            //Check that we have scores, otherwise blank out
            if (i < leaderboardInfo.Count)
            {
                leaderboardNames[i].text = leaderboardInfo[i].Key;
                leaderboardScores[i].text = leaderboardInfo[i].Value.ToString();
            }
            else
            {
                leaderboardNames[i].text = string.Empty;
                leaderboardScores[i].text = string.Empty;
            }
        }
    }
}
