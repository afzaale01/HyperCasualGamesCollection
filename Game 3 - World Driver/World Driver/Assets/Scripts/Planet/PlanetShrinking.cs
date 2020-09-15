using System.Collections;
using System.Collections.Generic;
using Lewis.MathUtils;
using Lewis.Score;
using UnityEngine;

[RequireComponent(typeof(PlanetData))]
public class PlanetShrinking : MonoBehaviour
{
    //Speed at which the planet shrinks
    [SerializeField]
    private float shrinkSpeed = 0.05f;

    //Store the planet data component
    private PlanetData planetData;

    //Size of the planet when the game starts
    private float planetStartSize;

    //Amount to mutiply the scale by to get the score
    private const float scoreMutiplier = 10;

    //If planet shrinking is allowed
    private bool allowPlanetShrink;


    private void Start()
    {
        planetData = GetComponent<PlanetData>();
        planetStartSize = planetData.PlanetSize;

        //Start the planet shrinking
        allowPlanetShrink = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (allowPlanetShrink)
        {
            //Shrink the planet, if we have a size greater than 0 
            if (planetData.PlanetSize > 0)
            {
                transform.localScale *= 1f - (shrinkSpeed * Time.deltaTime);

                //Increase Score
                float planetSizeDifference = Mathf.Abs(planetStartSize - planetData.PlanetSize);
                ScoreKeeper.CurrentScore = Mathf.Round(planetSizeDifference * scoreMutiplier);

            }
        }
    }

    /// <summary>
    /// Stop the planet from shrinking
    /// </summary>
    private void StopPlanetShrink()
    {
        allowPlanetShrink = false;
    }

    #region Event Subs/UnSubs
    private void OnEnable()
    {
        GameStateManager.LevelCompleted += StopPlanetShrink;
    }
    private void OnDisable()
    {
        GameStateManager.LevelCompleted -= StopPlanetShrink;
    }

    #endregion
}
