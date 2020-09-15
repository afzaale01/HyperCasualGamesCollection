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

    //Prefix before score in leaderboard
    private const string scorePrefix = "$";

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
                //Format the score using Standard numeric format strings - 
                //https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings?redirectedfrom=MSDN#NFormatString
                leaderboardScores[i].text = scorePrefix + leaderboardInfo[i].Value.ToString("N0");
            }
            else
            {
                leaderboardNames[i].text = string.Empty;
                leaderboardScores[i].text = string.Empty;
            }
        }
    }
}
