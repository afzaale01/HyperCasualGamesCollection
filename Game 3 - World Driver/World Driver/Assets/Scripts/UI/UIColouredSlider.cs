using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for sliders that change colour based on their values
/// </summary>
[RequireComponent(typeof(Slider))]
public class UIColouredSlider : MonoBehaviour
{
    //Store the slider that is attached to this script
    private Slider uiSlider;
    //Store the image of the slider that we will change the colour of
    private Image sliderImage;

    //The gradient of colours that we should use for the slider colours
    [SerializeField] private Gradient sliderGradient;


    private void Awake()
    {
        //Get the slider attached to this object. It is garenteed to have
        //one as we have [RequireComponent(typeof(Slider))]
        uiSlider = GetComponent<Slider>();

        //Get the Image component of the slider that we will change the colour of
        sliderImage = uiSlider.fillRect.GetComponent<Image>();
    }

    private void OnEnable()
    {
        //Subscribe to the on value changed function so that we update the slider colour
        //when its value is changed
        uiSlider.onValueChanged.AddListener(UpdateSliderColour);
    }

    private void OnDisable()
    {
        //Unsuscribe from the event when we are disabled so the event doesn't fire and
        //error our
        uiSlider.onValueChanged.RemoveListener(UpdateSliderColour);
    }

    /// <summary>
    /// Updates the slider colour based on its current % of it's max value
    /// Intended to be called from the inspector when the value of the slider
    /// is updated
    /// </summary>
    public void UpdateSliderColour(float newValue)
    {
        //Get the slider colour by getting the percentage that we are along the slider
        //then getting the colour at the same percentage in the gradient
        sliderImage.color = sliderGradient.Evaluate(newValue / (uiSlider.maxValue - uiSlider.minValue));
    }

    
}
