using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for a coloured health bar which changes colour based on health
/// </summary>
public class ColoredHealthbar : MonoBehaviour
{
    //Image used
    [SerializeField] private Image barImage;

    //Gradient for healthbard colour
    [SerializeField] private Gradient healthGradient;

    //Image properies
    [SerializeField]
    private float imageStartWidth = 500; //Image Width when we start (and this 100% health)

    /// <summary>
    /// Updates the bar image based on its current % of it's max value
    /// </summary>
    public void UpdateBarWidth(float newValue)
    {
        //Divide our width by our currect health
        RectTransform barRectTransform = barImage.transform as RectTransform;
        if (barRectTransform == null)
        {
            return;
        }

        float newImageWidth = imageStartWidth * (newValue / CarData.MaxHealth);
        barRectTransform.sizeDelta = new Vector2(newImageWidth, barRectTransform.sizeDelta.y);
    }

    /// <summary>
    /// Updates the bar colour based on its current % of it's max value
    /// </summary>
    public void UpdateBarColour(float newValue)
    {
        //Get the slider colour by getting the percentage that we are along the slider
        //then getting the colour at the same percentage in the gradient
        barImage.color = healthGradient.Evaluate((newValue / CarData.MaxHealth));
    }

    #region Event Subs/UnSubs

    private void OnEnable()
    {
        //When the player health updates make sure we update the UI
        //elements
        PlayerCarData.UpdatePlayerHealth += UpdateBarWidth;
        PlayerCarData.UpdatePlayerHealth += UpdateBarColour;
    }

    public void OnDisable()
    {
        PlayerCarData.UpdatePlayerHealth -= UpdateBarWidth;
        PlayerCarData.UpdatePlayerHealth -= UpdateBarColour;
    }

    #endregion
}
