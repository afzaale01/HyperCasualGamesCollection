using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton
/// Class is an over seer of the AI Cars
/// Keeps track of all of the AI cars that exist in the world
/// </summary>
public class AIOverseer : MonoBehaviour
{
    private static AIOverseer instance;

    //List of AI that are alive in the world
    private List<AICarData> aliveAIList;

    //Propery for the number of living AI
    public int AICount
    {
        get { return aliveAIList.Count; }
    }

    private void Start()
    {
        //Singleton, if we are created and an instance exists
        //then destroy
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;

        //Initalise our AI list
        aliveAIList = new List<AICarData>();
    }

    /// <summary>
    /// Get the singleton instance of the AI overseer, creates on if one
    /// does not already exist
    /// </summary>
    /// <returns></returns>
    public static AIOverseer GetInstance()
    {
        if (instance == null)
        {
            //Create an empty game object and add an AI overseer to it
            GameObject overseerGO = new GameObject("AI Overseer");
            return overseerGO.AddComponent<AIOverseer>();
        }

        return instance;
    }

    /// <summary>
    /// Add an AI car to the list of alive cars
    /// </summary>
    /// <param name="car">Car to Add</param>
    private void AddAICarToList(AICarData car)
    {
        //Check that the car doesn't already exist in the list
        //then add it if not
        if (!aliveAIList.Contains(car))
        {
            aliveAIList.Add(car);
        }
    }
    
    /// <summary>
    /// Remove an AI car from the alive list
    /// </summary>
    /// <param name="car">Car to Remove</param>
    private void RemoveAICarFromList(AICarData car)
    {
        //Check that the car is in the list, remove it if 
        //it is
        if (aliveAIList.Contains(car))
        {
            aliveAIList.Remove(car);
        }
    }

    #region Event Subs/Unsubs
    private void OnEnable()
    {
        AICarData.CarCreated += AddAICarToList;
        AICarData.CarDisabled += RemoveAICarFromList;
    }
    private void OnDisable()
    {
        AICarData.CarCreated -= AddAICarToList;
        AICarData.CarDisabled -= RemoveAICarFromList;
    }
    #endregion
}


