using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Level Data Scriptable Object. Stores level data saved from the level saver
/// </summary>
public class LevelData : ScriptableObject
{
    public string levelName;
    [HideInInspector] public Vector2Int levelSize;

    //Private backing for the level we have loaded from file, as serialised objects
    //cannot store arrays we load the level from a file and then in to this variable
    private int[,] loadedLevel;

    //Level Data (loaded from file on 1st use, saves to file on write)
    public int[,] LevelArray
    {
        set
        {
            loadedLevel = value;
            SaveLevelData(loadedLevel);
        }

        get { return loadedLevel ?? (loadedLevel = LoadLevelData()); }
    }

    /// <summary>
    /// Save all of the level data to a file
    /// </summary>
    /// <param name="levelData"></param>
    private void SaveLevelData(int[,] levelData)
    {
        string jsonData = JsonConvert.SerializeObject(loadedLevel);
        File.WriteAllText(GetLevelFilePath(), jsonData);
    }

    /// <summary>
    /// Get the level data from a file
    /// </summary>
    /// <returns></returns>
    private int[,] LoadLevelData()
    {
        #if UNITY_EDITOR
        string jsonData = File.ReadAllText(GetLevelFilePath());

        #elif UNITY_ANDROID

        WWW reader = new WWW(GetLevelFilePath());
        while(!reader.isDone){
        }
        string jsonData = reader.text;

        #endif


        return JsonConvert.DeserializeObject<int[,]>(jsonData);
    }

    /// <summary>
    /// Gets the filepath to save/load the level
    /// </summary>
    /// <returns></returns>
    private string GetLevelFilePath()
    {
        return Application.streamingAssetsPath + "/" + levelName + ".json";
    }
}
