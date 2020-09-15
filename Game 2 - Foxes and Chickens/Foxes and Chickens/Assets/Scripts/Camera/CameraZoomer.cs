using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomer : MonoBehaviour
{
    [SerializeField]
    private float camMoveSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        //Start the camera at the center point of the level
        transform.position = LevelGenerator.GetInstance().GetLevelCenter();
    }

    // Update is called once per frame
    void Update()
    {

        //Zoom the camera out while we cannot see all of the cubes
        if (!LevelGenerator.GetInstance().AllCubesVisible(GetComponent<Camera>()))
        {
            transform.position -= transform.forward * Time.deltaTime * camMoveSpeed;
        }
    }
}
