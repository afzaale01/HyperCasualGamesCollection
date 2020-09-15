using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Lewis.GameOptions
{

    /// <summary>
    /// Struct of game options that we can store
    /// </summary>
    [System.Serializable]
    public class GameOptionsInfo
    {
        public float musicVolume = 0.5f;
        public float sfxVolume = 0.5f;
        public string playerName = "Default Danny";
    }

    /// <summary>
    /// Class that stores the currently active game options
    /// </summary>
    public static class GameOptionsManager
    {
        //Save whether the game options have been loaded from file
        private static bool optionsLoaded;
        //Store the current game options so we don't need to load
        //from file every time
        private static GameOptionsInfo currentGameOptions;

        //File path where options are stored
        private static readonly string optionsSaveLocaton = Application.persistentDataPath + "GameOptions.json";

        //Events from when game elements should update because options
        //have been changed
        public delegate void GameOptionsEvent();
        public static event GameOptionsEvent GameOptionsUpdated;

        /// <summary>
        /// Get the current Game Options, from file if none are stored in memory
        /// or the last saved options values in memory
        /// </summary>
        /// <returns>Current Game Options</returns>
        public static GameOptionsInfo GetCurrentGameOptions()
        {
            //If we haven't yet loaded the game options from file,
            //load them
            if (!optionsLoaded)
            {
                currentGameOptions = LoadOptionsFromFile();
                optionsLoaded = true;
            }

            //If game options are null then assign the default game options
            if (currentGameOptions == null)
            {
                currentGameOptions = new GameOptionsInfo();
            }

            //Return current game options
            return currentGameOptions;
        }

        /// <summary>
        /// Save the current game options in memory and to file
        /// </summary>
        public static void SaveGameOptions(GameOptionsInfo optionsToSave)
        {
            //Save options in memory
            currentGameOptions = optionsToSave;

            //Save ootions to file
            SaveOptionsToFile(currentGameOptions);

            //Trigger event
            GameOptionsUpdated?.Invoke();
        }

        /// <summary>
        /// Load Game Options from file
        /// </summary>
        /// <returns></returns>
        private static GameOptionsInfo LoadOptionsFromFile()
        {
            //If we don't have an options file then return
            //the default null
            if (!File.Exists(optionsSaveLocaton))
            {
                return null;
            }

            //Load the options from the file, deserialize and return
            string loadData = File.ReadAllText(optionsSaveLocaton);
            GameOptionsInfo loadedOptions = JsonConvert.DeserializeObject<GameOptionsInfo>(loadData);
            return loadedOptions;
        }

        /// <summary>
        /// Save a set of game options to file
        /// </summary>
        private static void SaveOptionsToFile(GameOptionsInfo optionsToSave)
        {
            //Convert to JSON Format and save
            string saveData = JsonConvert.SerializeObject(optionsToSave, Formatting.Indented);
            File.WriteAllText(optionsSaveLocaton, saveData); 
        }

        public static string GetOptionsSaveLocation()
        {
            return optionsSaveLocaton;
        }
    }
}
