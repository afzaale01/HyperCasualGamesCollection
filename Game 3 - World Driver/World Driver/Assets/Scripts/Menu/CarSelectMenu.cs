using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Class to control the car select menu.
/// Allows the player to select different cars and displays the models
/// and stats of the selected car
/// </summary>
public class CarSelectMenu : Menu
{
    //List cars that can be selected
    [SerializeField] private List<CarInfo> selectableCars;

    //Currently Selected Car
    private int selectedCarIndex;

    //UI Elements for showing states
    [SerializeField] private Transform carModelDisplay;
    [SerializeField] private TextMeshProUGUI carNameDisplay;
    [SerializeField] private Slider carSpeedSlider;
    [SerializeField] private Slider carHandlingSlider;

    // Start is called before the first frame update
    private void Start()
    {
        //Intialise the selected car index
        selectedCarIndex = 0;

        if (selectableCars.Count > 0)
        {
            //Set to 1st car in list
            SetUIToCar(selectableCars[selectedCarIndex]);

            //Set the game state manager to know which car we have selected so that the 
            //we can load it when we get in to the game
            GameStateManager.PlayerSelectedCar = selectableCars[selectedCarIndex];
        }
    }

    /// <summary>
    /// Change the selected car and set the UI to the new cars values.
    /// Intended to be called from the UI Button
    /// </summary>
    /// <param name="indexChange">Amount to change the index of the selected car (i.e 1 will select the
    /// next car in the list and -1 will select the previous car in the list)</param>
    // ReSharper disable once UnusedMember.Global
    public void ChangeSelectedCar(int indexChange)
    {
        //Increment Selected Car Index and if it will excede the number
        //of cars that we have to select wrap it around to 0
        selectedCarIndex += indexChange;
        selectedCarIndex = selectedCarIndex >= selectableCars.Count ? 0 : selectedCarIndex;
        selectedCarIndex = selectedCarIndex >= 0 ? selectedCarIndex : (selectableCars.Count - 1);

        //Set the ui to show the selected car
        SetUIToCar(selectableCars[selectedCarIndex]);

        //Set the game state manager to know which car we have selected so that the 
        //we can load it when we get in to the game
        GameStateManager.PlayerSelectedCar = selectableCars[selectedCarIndex];
    }

    /// <summary>
    /// Set the UI elements to the given car.
    /// Displaying the cars stats
    /// </summary>
    /// <param name="carToSet"></param>
    private void SetUIToCar(CarInfo carToSet)
    {
        //Check that the car we have been giving is valid,
        //otherwise early out
        if (carToSet == null)
        {
            return;
        }

        //Remove old car model if one exists
        if (carModelDisplay.childCount > 0)
        {
            foreach (Transform child in carModelDisplay)
            {
                Destroy(child.gameObject);
            }
        }

        //Create a new car model and set its transform to the correct position
        GameObject newCar = Instantiate(carToSet.model, carModelDisplay.transform, false);
        newCar.transform.localPosition = Vector3.zero;

        //Set Name
        carNameDisplay.text = carToSet.name;

        //Set Stats Sliders
        carSpeedSlider.value = carToSet.speedMutiplier;
        carHandlingSlider.value = carToSet.handilingMutiplier;
    }
}
