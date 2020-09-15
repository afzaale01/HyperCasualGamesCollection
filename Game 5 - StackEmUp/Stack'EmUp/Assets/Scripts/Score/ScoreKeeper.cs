using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lewis.Score
{
    /// <summary>
    /// Score keeper class, simply saves the current score
    /// </summary>
    public static class ScoreKeeper
    {
        //Event for when score is updated so that we update UI
        public delegate void ScoreUpdateEvent(float newScore);
        public static event ScoreUpdateEvent UpdateScoreUI;

        private static float currentScore = 0;
        private static float minScore = 0;
        private static float maxScore = Mathf.Infinity;
        public static float CurrentScore
        {
            get { return currentScore; }
            set { 
                currentScore = Mathf.Clamp(value, minScore, maxScore); /*Cap Score*/
                UpdateScoreUI?.Invoke(currentScore);
            }
        }

        public static void ResetScore()
        {
            currentScore = minScore;
            UpdateScoreUI?.Invoke(CurrentScore);
        }

    }
}
