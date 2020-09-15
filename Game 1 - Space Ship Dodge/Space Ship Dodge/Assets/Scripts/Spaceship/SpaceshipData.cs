using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lewis.Score;

/// <summary>
/// Data about spaceship
/// Health, Coins, Effects etc
/// </summary>
public class SpaceshipData : MonoBehaviour
{

    //Animation Controller
    [Header("Animation")]
    [SerializeField]
    private Animator invunerableAnimation;
    [SerializeField] private GameObject shipModel;
    [SerializeField] private GameObject shipParticles;

    [Header("Death/Damage Settings")] 
    //Amount of score to lose when the player takes damage
    [SerializeField] private int damageScoreReduction = 0;

    //Presets
    private const float playerStartHealth = 4;
    private const float playerMaxHealth = 4;
    private const float playerInvunerableTime = 2f;
    private const float playerCollisionDamage = 1f;

    //Active Player Stats
    private float playerHealth = playerStartHealth;
    public float PlayerHealth { get { return playerHealth; } }
    private bool isInvunerable = false;


    private void Start()
    {
        //Stop Inv Animation
        invunerableAnimation.enabled = false;
    }

    #region Health/Death

    ///Events for player taking damage / being given health
    public delegate void HealthEvent(float health);
    public delegate void InvunverableEvent(float time);
    public delegate void DeathEvent();
    public static event HealthEvent PlayerDamaged;
    public static event HealthEvent PlayerHealed;
    public static event DeathEvent PlayerKilled;
    public static event InvunverableEvent PlayerMadeInvunerable;

    /// <summary>
    /// Damage the Player, if not invunverable
    /// </summary>
    /// <param name="damage">Health to remove from the player</param>
    public void DamagePlayer(float damage)
    {
        if (!isInvunerable)
        {
            //Reduce Score when damaged
            ScoreKeeper.CurrentScore -= damageScoreReduction;

            //Calculate the resultant health and make sure that it doesn't
            //go below zero
            float resultantHealth = playerHealth - damage;

            if (resultantHealth > 0)
            {
                //Call damage events and functionality
                playerHealth = resultantHealth;
                PlayerDamaged?.Invoke(playerHealth);

                //Set player to be invunerable for an amount of time
                //so they don't take damage 100 times while clipping through a wall
                StartCoroutine(DoInvunerableSequence(playerInvunerableTime));
            }
            else
            {
                //Call game over/death events
                playerHealth = 0;
                PlayerKilled?.Invoke();
            }
        }
    }
   

    /// <summary>
    /// Add Health to the player
    /// </summary>
    /// <param name="healthToAdd">Health to add to the player</param>
    private void AddHealth(float healthToAdd)
    {
        //Set the players health to either the new health or
        //the max health
        float resultantHealth = playerHealth + healthToAdd;
        playerHealth = Mathf.Min(resultantHealth, playerMaxHealth);

        //Call Event
        PlayerHealed?.Invoke(playerHealth);
    }


    /// <summary>
    /// Set the player to be invurentable for an amount of time
    /// </summary>
    /// <param name="time">Time to be Invunerable</param>
    private IEnumerator DoInvunerableSequence(float time)
    {
        //Enable Invunderability
        isInvunerable = true;
        invunerableAnimation.enabled = true;
        PlayerMadeInvunerable?.Invoke(time);

        //Wait
        yield return new WaitForSeconds(time);

        //Disable Invunerability
        isInvunerable = false;
        invunerableAnimation.enabled = false;
        //Make sure we are visible after the animation
        if (shipModel)
        {
            shipModel.SetActive(true);
        }
        if (shipParticles)
        {
            shipParticles.SetActive(true);
        }
    }

    #endregion

    #region Pickups

    /// <summary>
    /// Register a coin
    /// </summary>
    /// <param name="coin">Coin picked up</param>
    private void PickupCoin(CoinPickup coin)
    {
        //Add to score and destroy coin
        ScoreKeeper.CurrentScore += coin.Value;
        coin.RegisterPickup();
    }

    #endregion

    /// <summary>
    /// Resets all data for the ship
    /// (i.e health)
    /// </summary>
    public void ResetShipData()
    {
        playerHealth = playerStartHealth;
        isInvunerable = false;
    }

    //Checking for collision with walls or bullets
    private void OnTriggerEnter(Collider other)
    {
        //Check if we have collided with a obstacle and damage
        //the player
        DestructableWall destructableWall = other.GetComponent<DestructableWall>();
        if (destructableWall)
        {
            //Damage the player
            DamagePlayer(playerCollisionDamage);

            //Destroy the wall
            destructableWall.ExplodeWall();
            return;
        }

        //Check if the player has collided with a pickup
        CoinPickup pickup = other.GetComponent<CoinPickup>();
        if (pickup)
        {
            PickupCoin(pickup);
        }
    }

    #region Events Sub/UnSub
    private void OnEnable()
    {
        GameStateManager.LevelStarted += ResetShipData;
    }

    private void OnDisable()
    {
        GameStateManager.LevelStarted -= ResetShipData;
    }

    #endregion
}
