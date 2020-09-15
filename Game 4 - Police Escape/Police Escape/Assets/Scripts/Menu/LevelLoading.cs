using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class that has defines for all of the level IDs and level names
/// </summary>
public static class LevelLoading
{
    //Dictonary to store levels and level names
    //<LEVEL ID, LEVEL NAME>
    private static readonly Dictionary<int, string> levelNamesDictonary = new Dictionary<int, string>()
    {
        { 1, "SCN_TestingScene" }, //Example content
    };

    /// <summary>
    /// Gets the level ID from the scene name
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    public static int GetLevelIDFromName(string sceneName)
    {
        //Search each item in the dictonary for the scene name we are looking for 
        //and get it's key (i.e level id)
        if (levelNamesDictonary.ContainsValue(sceneName))
        {
            int levelID = levelNamesDictonary.First(x => x.Value == sceneName).Key;
            return levelID;
        }

        //No level exists return -1
        return -1;
    }

    public static string GetLevelSceneName(int levelID) {

        //Check if the dictonary contains the level id
        if (levelNamesDictonary.ContainsKey(levelID))
        {
            return levelNamesDictonary[levelID];
        }

        //No Level found return empty string
        return string.Empty;
    }

}
