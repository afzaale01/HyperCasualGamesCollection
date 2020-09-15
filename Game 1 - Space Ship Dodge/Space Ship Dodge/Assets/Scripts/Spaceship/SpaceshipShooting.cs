using System.Collections;
using System.Collections.Generic;
using Lewis.GameOptions;
using UnityEngine;
using Lewis.TouchExtensions;

/// <summary>
/// Dealing with the player shooting from thier ship
/// </summary>
public class SpaceshipShooting : MonoBehaviour
{
    [Header("Audio")] 
    [SerializeField] private AudioClip shootingSound;
    [SerializeField] private SFXAudioSource shootingAudioSource;


    [Header("Shooting Settings")]
    //Bullet Prefab
    [SerializeField]
    private GameObject bulletPrefab;

    //Min amount of time between shots
    [SerializeField]
    private float shotSpacing = 0.0f;
    private float shotTimer = 0.0f; //Timer for shooting, if <0 then shot is allowed

    //Point(s) to create bullets at, local positions
    [SerializeField]
    private Vector3[] bulletSpawns;

    private bool shootingEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        //Verify bullet prefab is correct
        if (!bulletPrefab.GetComponent<ShipBullet>())
        {
            Debug.LogWarning("Spaceship Shooting bullet prefab does not have connected bullet script");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (shootingEnabled)
        {
            //Check for shot touch
            if (TouchInput.TouchIsValid(TouchDefines.shotTouchID))
            {
                //If there has been enough time since the last shot then create a new bullet
                if(shotTimer < 0)
                {
                    //Spawn bullets at all of the bullet spawn points
                    for (int i = 0; i < bulletSpawns.Length; i++)
                    {
                        //Create Bullet, set it's postion and direction
                        ShipBullet createdBullet = Instantiate(bulletPrefab).GetComponent<ShipBullet>();
                        createdBullet.transform.position = transform.TransformPoint(bulletSpawns[i]);
                        createdBullet.transform.rotation = gameObject.transform.rotation;
                        createdBullet.BulletCreator = gameObject;
                    }

                    //Play Shooting Sound
                    if (shootingAudioSource && shootingSound)
                    {
                        shootingAudioSource.PlaySFX(shootingSound);
                    }

                    //Reset Shot Timer
                    shotTimer = shotSpacing;
                }
            }

            //Decrease shot time
            shotTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Reset Shooting for this ship
    /// </summary>
    public void ResetShooting()
    {
        shootingEnabled = true;
        shotTimer = 0.0f;
    }

    /// <summary>
    /// Stop this ship from shooting
    /// </summary>
    public void LockShooting()
    {
        shootingEnabled = false;
    }


    #region Events Sub/Unsub
    private void OnEnable()
    {
        GameStateManager.LevelStarted += ResetShooting;
        GameStateManager.LevelUnPaused += ResetShooting;
        GameStateManager.LevelCompleted += LockShooting;
        GameStateManager.LevelPaused += LockShooting;
        GameStateManager.LevelFailed += LockShooting;
    }
    private void OnDisable()
    {
        GameStateManager.LevelStarted -= ResetShooting;
        GameStateManager.LevelUnPaused -= ResetShooting;
        GameStateManager.LevelCompleted -= LockShooting;
        GameStateManager.LevelPaused -= LockShooting;
        GameStateManager.LevelFailed -= LockShooting;
    }
    #endregion

}
