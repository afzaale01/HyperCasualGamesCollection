using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Runtime;
using UnityEngine;
using Lewis.Camera;
using Random = UnityEngine.Random;


/// <summary>
/// Class for Spawning Police Car AI around the Map
/// </summary>
public class AISpawner : MonoBehaviour
{
    //Prefab of the car to spawn
    [Header("Physical Spawning")]
    [SerializeField]
    private GameObject aiCarPrefab;

    //Spawn points within the level
    [SerializeField]
    private Transform[] spawnPoints;
    //Rotation offset to spawn the vehicles
    private readonly Quaternion rotationOffset = Quaternion.Euler(0, 90, 0);

    [Header("Spawn Settings")] 
    [SerializeField] private int maxAICount; //Maximum number of AI irrespective of difficulty
    [SerializeField] private int baseAITarget; //Number of AI to spawn at difficulty 1.0
    [SerializeField] private int maxCarsPerSpawn = 1; //Maximum number of cars to spawn in spawn update
    [SerializeField] private float spawnInterval; //Time between spawning cars
    //Property for the current AI Target
    private int CurrentAITarget
    {
        get
        {
            float aiTarget = baseAITarget * GameStateManager.GameDifficulty;
            aiTarget = Mathf.Clamp(aiTarget, 0, maxAICount);
            return Mathf.CeilToInt(aiTarget);
        }
    }

    private AIOverseer aiController; //AI overseer is used to get the AI count
    private float spawnTimer; //Time since last spawn

    [SerializeField] private Camera playerCamera;

    [Header("Debug")]
    //Mesh we draw at the spawn position for the car
    [SerializeField] private Mesh debugDrawMesh;

    // Start is called before the first frame update
    void Start()
    {
        spawnTimer = 0f;

        //Get the overseer instance so we know it is created before we start spawning cars
        aiController = AIOverseer.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnTimer >= spawnInterval)
        {
            DoSpawnerUpdate();
            spawnTimer = 0f;
        }

        spawnTimer += Time.deltaTime;
    }

    /// <summary>
    /// Does a spawner update spawning cars if they are needed
    /// </summary>
    private void DoSpawnerUpdate()
    {
        //Check if we need to spawn a car
        if (aiController.AICount < CurrentAITarget)
        {
            //Get how many cars we should spawn, either the max number we should spawn
            //or how many are required if it is below the max to spawn
            int carsToSpawn = Math.Min((CurrentAITarget - aiController.AICount), maxCarsPerSpawn);

            for (int i = 0; i <= carsToSpawn; ++i)
            {
                //Get all of the non visible spawn points and spawn a car at one of them
                List<Transform> potentialSpawnPoints = GetNonVisibleSpawnPoints();
                if (potentialSpawnPoints != null)
                {
                    if (potentialSpawnPoints.Count > 0)
                    {
                        int randomSpawnPointIndex = Random.Range(0, potentialSpawnPoints.Count);
                        SpawnAICar(potentialSpawnPoints[randomSpawnPointIndex]);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Spawn an AI car at a given spawn point
    /// </summary>
    /// <param name="spawnPoint">Point to Spawn at</param>
    private void SpawnAICar(Transform spawnPoint)
    {
        GameObject newCar = Instantiate(aiCarPrefab);
        newCar.transform.position = spawnPoint.position;
        newCar.transform.rotation = spawnPoint.rotation * rotationOffset;
    }

    /// <summary>
    /// Gets all of the spawn points that are not visible from the player
    /// </summary>
    /// <returns>List of spawn points that are not visble to the player camera</returns>
    List<Transform> GetNonVisibleSpawnPoints()
    {
        List<Transform> nonVisiblePoints = new List<Transform>();
        foreach (Transform currentPoint in spawnPoints)
        {
            if (!CameraUtils.IsPointWithinCamView(currentPoint.position, Camera.current))
            {
                nonVisiblePoints.Add(currentPoint);
            }
        }

        return nonVisiblePoints;
    }

    /// <summary>
    /// Disable car spawning
    /// </summary>
    private void DisableSpawning()
    {
        this.enabled = false;
    }


    //Debug Drawing of Spawn Positions
    private void OnDrawGizmosSelected()
    {
        if (aiCarPrefab != null && debugDrawMesh != null)
        {
            foreach (Transform point in spawnPoints)
            {
                if (point != this.transform)
                {
                    //Draw Car at Spawn Position
                    Gizmos.color = Color.yellow;
                    Vector3 meshScale = new Vector3(100, 100, 100);
                    Gizmos.DrawWireMesh(debugDrawMesh, point.position, point.rotation, meshScale);
                }
            }
        }
    }

    #region Event Subs/Unsubs

    private void OnEnable()
    {
        //Sub to disable spawning then the player is killed
        PlayerCarData.PlayerKilled += DisableSpawning;
    }

    private void OnDisable()
    {
        PlayerCarData.PlayerKilled -= DisableSpawning;
    }

    #endregion


}
