using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Lewis.Score
{

    //Maybe Convert this to use custom class instead of dictonary so that we can do dates and stuff
    //[or use a List of Key Value Pairs]

    /// <summary>
    /// Class to deal with the saving of scores as well as leaderboards
    /// </summary>
    public static class ScoreLocalLeaderboards
    {
        private const int currentSaveFileVersion = 1;

        [System.Serializable]
        private class ScoreSaveData
        {
            public int saveVersion = currentSaveFileVersion; //Save file version so that we can don't try and read old save file versions
            public List<KeyValuePair<string, float>> scores = new List<KeyValuePair<string, float>>();
        }

        //Number of top scores to save for local leaderboards
        private const int localScoreCountLimit = 10;

        private const string scoreFilePrefx = "LeaderboardScores-";

        /// <summary>
        /// Adds a score to the leaderboard save file
        /// </summary>
        /// <param name="nameToAdd">Name to add new score under</param>
        /// <param name="scoreToAdd">Score of new Score</param>
        /// <param name="levelName">Name of level to save score for</param>
        /// <param name="overrideScoreCountLimit">Add score if even if it excedes the number of allowed scores to save</param>
        /// <returns>If score was added to file</returns>
        public static bool AddScore(string nameToAdd, float scoreToAdd, string levelName, bool overrideScoreCountLimit = false)
        {
            //Get current score infomation
            ScoreSaveData loadedScoresInfo = LoadScoresInfo(levelName);

            //Check if we have less than the score count limit and can just save the score to the file
            //or we are overriding the limit
            if(loadedScoresInfo.scores.Count <= localScoreCountLimit || overrideScoreCountLimit)
            {
                loadedScoresInfo.scores.Add(new KeyValuePair<string, float>(nameToAdd, scoreToAdd));
                SaveScoresInfo(loadedScoresInfo, levelName);
                return true;
            }
            else
            {

                //Work out if this score is greater than the lowest score, if it is
                //remove the last value and add this new value to the list
                if(scoreToAdd >= loadedScoresInfo.scores[loadedScoresInfo.scores.Count - 1].Value)
                {
                    loadedScoresInfo.scores.Remove(loadedScoresInfo.scores[loadedScoresInfo.scores.Count - 1]);
                    loadedScoresInfo.scores.Add(new KeyValuePair<string, float>(nameToAdd, scoreToAdd));
                    SaveScoresInfo(loadedScoresInfo, levelName);
                    return true;
                }
            }

            return false;

        }

        /// <summary>
        /// Gets a list of the saved scores
        /// </summary>
        /// <returns></returns>
        public static List<KeyValuePair<string, float>> GetSortedScores(string levelName)
        {
            return SortScores(LoadScoresInfo(levelName).scores);
        }

        /// <summary>
        /// Gets Scores in order with position (i.e 1st, 2nd, 3rd etc)
        /// </summary>
        /// <returns>Sorted Dictonary of scores in order</returns>
        public static SortedDictionary<int, KeyValuePair<string, float>> GetScoresWithPosition(string levelName)
        {
            List<KeyValuePair<string, float>> sortedScores = GetSortedScores(levelName);
            SortedDictionary<int, KeyValuePair<string, float>> scorePosDictonary = new SortedDictionary<int, KeyValuePair<string, float>>();

            //Loop through each of the scores and assign it a position
            for (int i = 0; i < sortedScores.Count(); i++)
            {
                //Add current scores to dictonary at score pos in array + 1 (because we index at 0)
                scorePosDictonary.Add(i + 1, sortedScores[i]);
            }

            return scorePosDictonary;
            
        }

        /// <summary>
        /// Saves score info to scores file
        /// </summary>
        /// <param name="data">Score data to save</param>
        private static void SaveScoresInfo(ScoreSaveData data, string levelName)
        {
            //Sort Scores
            data.scores = SortScores(data.scores);
            string saveData = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(GetScoreSaveLocation(levelName), saveData);
        }

        /// <summary>
        /// Load the score data from a file and get it in a formatted way
        /// </summary>
        /// <returns>Loaded Score Data</returns>
        private static ScoreSaveData LoadScoresInfo(string levelName)
        {
            string fileReadData = String.Empty;

            //Read data from file
            if (File.Exists(GetScoreSaveLocation(levelName)))
            {
                fileReadData = File.ReadAllText(GetScoreSaveLocation(levelName));
                
            }

            //Check for empty or invalid file
            if (fileReadData.Length == 0)
            {
                return new ScoreSaveData();
            }

            return JsonConvert.DeserializeObject<ScoreSaveData>(fileReadData);
        }

        /// <summary>
        /// Sort a list of scores
        /// </summary>
        /// <param name="scores">Score List (of pairs (name, score))</param>
        /// <returns></returns>
        private static List<KeyValuePair<string, float>> SortScores(List<KeyValuePair<string, float>> scores)
        {
            scores.Sort((x, y) => (y.Value.CompareTo(x.Value)));
            return scores;
        }

        /// <summary>
        /// Get the location to store a particular levels leaderboard info at
        /// </summary>
        /// <returns>Location of score file</returns>
        public static string GetScoreSaveLocation(string levelName)
        {
            return Application.persistentDataPath +  "/" + scoreFilePrefx + levelName;
        }
    }
}
