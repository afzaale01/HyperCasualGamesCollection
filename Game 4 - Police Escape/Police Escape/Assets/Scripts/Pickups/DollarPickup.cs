using System.Collections;
using System.Collections.Generic;
using Lewis.Score;
using UnityEngine;

/// <summary>
/// Class for a dollar pickup
/// </summary>
public class DollarPickup : MonoBehaviour
{
    [SerializeField]
    //Number of $ this pickup adds to the score when our difficulty is 1.0
    private float baseScoreValue = 25f;

    //Time to disable this pickup for after pickup
    [SerializeField] private float disableTime = 5f;

    //Rendered Model/Animation
    [SerializeField] private GameObject dollarModel;
    //Box Collider (used to detect if we enter this obj)
    [SerializeField] private BoxCollider dollarCollider;
    private bool disabled;

    private void Start()
    {
        //Set not disabled
        disabled = false;

        //Random Chance to start disabled
        int startDisabled = Random.Range(0, 2);
        if (startDisabled == 1)
        {
            disabled = true;
            StartCoroutine(DisablePickup(disableTime));
        }
        else
        {
            //Set not disabled
            disabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerMovement>())
        {
            //If we are not disabled add score and disable this object
            if (disabled == false)
            {
                ScoreKeeper.CurrentScore += Mathf.Ceil(baseScoreValue * GameStateManager.GameDifficulty);
                StartCoroutine(DisablePickup(disableTime));
            }
        }
    }

    /// <summary>
    /// Disable the pickup for a number of seconds
    /// </summary>
    /// <param name="seconds">Number of seconds to disable for</param>
    private IEnumerator DisablePickup(float seconds)
    {
        //Check for valid model & collider
        if (!dollarModel || !dollarCollider)
        {
            yield break;
        }

        //Disable to model and collider
        dollarModel.SetActive(false);
        dollarCollider.enabled = false;
        disabled = true;

        yield return new WaitForSeconds(seconds);

        //Reenable
        dollarModel.SetActive(true);
        dollarCollider.enabled = true;
        disabled = false;

    }

    /// <summary>
    /// Destroy the pickup
    /// Intended to be invoked by player killed event
    /// </summary>
    private void DestroyPickup()
    {
        Destroy(gameObject);
    }

    #region Event Subs/Unsubs

    private void OnEnable()
    {
        PlayerCarData.PlayerKilled += DestroyPickup;
    }

    private void OnDisable()
    {
        PlayerCarData.PlayerKilled -= DestroyPickup;
    }

    #endregion

}
