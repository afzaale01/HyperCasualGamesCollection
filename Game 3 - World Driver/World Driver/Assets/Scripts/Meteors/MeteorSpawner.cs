using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorSpawner : MonoBehaviour
{
    //Min/Max time between metor spawning
    [SerializeField]
    private float minMeteorSpawnTime, maxMeteorSpawnTime;

    [SerializeField]
    private GameObject meteorPrefab;

    //Radius away from the planet that we should spawn
    //the meteor
    [SerializeField]
    private float meteorSpawnRadius = 20f;

    //Timer to keep track of time between meteor spawns
    private float meteorTimer;


    // Start is called before the first frame update
    private void Start()
    {
        //Set the first meteor to spawn after the max time so that we give the player
        //the best opertunity to get aqanted with the controls
        meteorTimer = maxMeteorSpawnTime;   
    }

    // Update is called once per frame
    private void Update()
    {
        //Decrease the meteor timer
        meteorTimer -= Time.deltaTime;

        //When the meteor timer reduces to 0 then
        //spawn a new meteor and reset the timer
        if(meteorTimer <= 0f)
        {
            SpawnMeteor();

            //Reset timer to a random value between the min and max
            meteorTimer = Random.Range(minMeteorSpawnTime, maxMeteorSpawnTime);
        }
    }

    /// <summary>
    /// Spawns a Meteor at a random position around the planet
    /// </summary>
    private void SpawnMeteor()
    {
        //Get spawn location - choose a random point on a sphere and
        //then mutiply that by the radius to get a position outside the planet
        Vector3 spawnLocation = Random.onUnitSphere * meteorSpawnRadius;
        GameObject newMeteor = Instantiate(meteorPrefab);
        newMeteor.transform.position = spawnLocation;
    }
}
