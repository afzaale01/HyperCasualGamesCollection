using System.Collections;
using System.Collections.Generic;
using System.Data;
using Lewis.MathUtils;
using Lewis.Score;
using UnityEngine;

/// <summary>
/// Class for spawning objects in the world to fall
/// on to the platform
/// </summary>
public class ObjectSpawner : MonoBehaviour
{
    [Header("Objects")]
    //List of objects we can spawn
    [SerializeField] private List<GameObject> spawnableObjectPrefabs;

    [Header("Spawn Position")]
    //Position we should spawn the objects at
    [SerializeField] private Vector3 minSpawnPos;
    [SerializeField] private Vector3 maxSpawnPos;

    [Header("Spawn Limits")]
    //Limits to lock axies when spawned
    [SerializeField] private Vector3Int movementLimits;
    [SerializeField] private Vector3Int rotationLimits;

    [Header("Spawn Timing")]
    //Range of the spawn interval
    [SerializeField] private float minSpawnTime = 1f;
    [SerializeField] private float maxSpawnTime = 2f;
    //Current Time we are waiting for
    private float nextSpawnTime;
    //Timer to track since our last object spawn
    private float timeSinceLastSpawn;

    [Header("Score")] 
    [SerializeField] private bool increaseScore = true;

    //Player and audio clip for when we spawn
    [Header("Audio")] 
    [SerializeField] private SFXAudioSource spawnAudioSource;
    [SerializeField] private AudioClip spawnSound;

    //If spawing is enabled
    private bool allowSpawning;

    // Start is called before the first frame update
    void Start()
    {
        //Inialise Timers
        ResetSpawnTimers();

        //Allow Spawning
        allowSpawning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (allowSpawning)
        {
            timeSinceLastSpawn += Time.deltaTime;

            //If we are over spawn time then spawn an object
            if (timeSinceLastSpawn >= nextSpawnTime)
            {
                SpawnObject();
                ResetSpawnTimers();

                //Play spawn Sound
                if (spawnAudioSource && spawnSound)
                {
                    spawnAudioSource.PlaySFX(spawnSound);
                }

                if (increaseScore)
                {
                    //Increase Game Score when we spawn a new object
                    ScoreKeeper.CurrentScore++;
                }
            }
        }
    }

    /// <summary>
    /// Resets the spawn timers
    /// </summary>
    private void ResetSpawnTimers()
    {
        //Set time since spawn to 0
        timeSinceLastSpawn = 0f;
        //Randomise next spawn time
        nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
    }

    /// <summary>
    /// Spawn an object for the player to stack
    /// </summary>
    private void SpawnObject()
    {
        //Check we have objects to spawn
        if (spawnableObjectPrefabs.Count <= 0)
        {
            Debug.LogWarning("Cannot Spawn Objects as spawnableObjectPrefabs has no prefabs");
            return;
        }

        //Choose a random object
        int spawnObjectIndex = Random.Range(0, spawnableObjectPrefabs.Count);
        GameObject spawnObject = spawnableObjectPrefabs[spawnObjectIndex];
        if (spawnObject == null)
        {
            return;
        }

        //Choose a point between the range
        Vector3 spawnPos = MathUtils.RangeVec3(minSpawnPos, maxSpawnPos);

        //Set Limits for Rigidbody
        RigidbodyConstraints spawnedObjConstraints = RigidbodyConstraints.None;
        //Postition
        spawnedObjConstraints = movementLimits.x == 1 ? spawnedObjConstraints | RigidbodyConstraints.FreezePositionX : spawnedObjConstraints;
        spawnedObjConstraints = movementLimits.y == 1 ? spawnedObjConstraints | RigidbodyConstraints.FreezePositionY : spawnedObjConstraints;
        spawnedObjConstraints = movementLimits.z == 1 ? spawnedObjConstraints | RigidbodyConstraints.FreezePositionZ : spawnedObjConstraints;
        //Rotation
        spawnedObjConstraints = rotationLimits.x == 1 ? spawnedObjConstraints | RigidbodyConstraints.FreezeRotationX : spawnedObjConstraints;
        spawnedObjConstraints = rotationLimits.y == 1 ? spawnedObjConstraints | RigidbodyConstraints.FreezeRotationY : spawnedObjConstraints;
        spawnedObjConstraints = rotationLimits.z == 1 ? spawnedObjConstraints | RigidbodyConstraints.FreezeRotationZ : spawnedObjConstraints;

        //Create a object at the spawn position
        GameObject spawnedObject = Instantiate(spawnObject, spawnPos, Quaternion.identity);

        //Apply RB Limits if we have one
        Rigidbody spawnedRB = spawnedObject.GetComponent<Rigidbody>();
        if (spawnedRB)
        {
            spawnedRB.constraints = spawnedObjConstraints;
        }
    }

    /// <summary>
    /// Enable Spawning
    /// </summary>
    private void EnableSpawning()
    {
        allowSpawning = true;
    }

    /// <summary>
    /// Disable Spawning
    /// </summary>
    private void DisableSpawning()
    {
        allowSpawning = false;
    }

    #region Event Subs / Unsubs

    private void OnEnable()
    {
        GameStateManager.LevelPaused += DisableSpawning;
        GameStateManager.LevelCompleted += DisableSpawning;
        GameStateManager.LevelFailed += DisableSpawning;
        GameStateManager.LevelUnPaused += EnableSpawning;
    }

    private void OnDisable()
    {
        GameStateManager.LevelPaused -= DisableSpawning;
        GameStateManager.LevelCompleted -= DisableSpawning;
        GameStateManager.LevelFailed -= DisableSpawning;
        GameStateManager.LevelUnPaused -= EnableSpawning;
    }


    #endregion
}
