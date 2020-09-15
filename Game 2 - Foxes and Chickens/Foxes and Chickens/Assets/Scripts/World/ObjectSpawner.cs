using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : WalkableTile
{
    [SerializeField]
    private GameObject objectPrefab;

    // Start is called before the first frame update
    void Start()
    {
        //Create the object at this tiles position
        GameObject player = Instantiate(objectPrefab);
        player.transform.position = transform.position;   
    }
}
