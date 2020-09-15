using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelButton : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnEnable()
    {
        //Verify that the next level does exist,
        //if it doesn't destroy this button
        if(GetNextLevelName() == string.Empty)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Check if the next level exists
    /// </summary>
    /// <returns></returns>
    private string GetNextLevelName()
    {
        //Get the current level id from the scene name
        int currentLevelID = LevelLoading.GetLevelIDFromName(SceneManager.GetActiveScene().name);
        return LevelLoading.GetLevelSceneName(currentLevelID + 1);
    }

    /// <summary>
    /// Load the next game level
    /// </summary>
    public void LoadNextLevel()
    {
        string nextLevelName = GetNextLevelName();
        if(nextLevelName == string.Empty)
        {
            return;
        }

        SceneManager.LoadScene(nextLevelName);
    }
}
