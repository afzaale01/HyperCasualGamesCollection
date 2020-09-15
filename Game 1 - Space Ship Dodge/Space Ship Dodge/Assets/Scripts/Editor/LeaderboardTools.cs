using System.Collections;
using System.Collections.Generic;
using System.IO;
using Lewis.Score;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

/// <summary>
/// Editor extension to reset the leaderboard
/// </summary>
public class LeaderboardTools : MonoBehaviour
{

    /// <summary>
    /// Resets the leaderboard file
    /// </summary>
    [MenuItem("Game IO/Leaderboards/Reset Leaderboard")]
    public static void DeleteLeaderboardFile()
    {
        string leaderboardFile = ScoreLocalLeaderboards.GetScoreSaveLocation(SceneManager.GetActiveScene().name);
        if (File.Exists(leaderboardFile))
        {
            File.Delete(leaderboardFile);
            Debug.Log("[Success] Reset all Leaderboard scores from file: " + leaderboardFile);
        }
        else
        {
            Debug.LogWarning("[Failed] Could not delete leaderboard file. Does not exist or is in use: " + leaderboardFile);
        }


    }
}
