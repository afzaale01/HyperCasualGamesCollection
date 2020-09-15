using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class LevelMenuItem : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI levelIDText;

    //Level Values
    private int levelID;

    /// <summary>
    /// Initalise the Level Button
    /// </summary>
    /// <param name="levelIDToSet">ID this this level</param>
    /// <param name="creatingMenu">Menu that created this button</param>
    public void InitLevelButton(int levelIDToSet, LevelSelectMenu creatingMenu)
    {
        //Set ID/Text
        levelID = levelIDToSet;
        levelIDText.text = levelID.ToString();

        //Add Button on click
        Button button = GetComponent<Button>();
        button.onClick.AddListener(LoadLevel);
    }

    /// <summary>
    /// Loads this Level
    /// </summary>
    public void LoadLevel()
    {
        //Get the scene of this level and try and load it
        string levelName = LevelLoading.GetLevelSceneName(levelID);
        if (levelName != string.Empty)
        {
            SceneManager.LoadScene(levelName);
        }
    }
}
