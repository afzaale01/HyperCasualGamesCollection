using System.Collections;
using System.Collections.Generic;
using Lewis.MathUtils;
using UnityEngine;

public class Crater : GravityAffectedObject
{
    [SerializeField] private float shrinkSpeed; //Amount to shrink per second
    [SerializeField] private float destroySize; //Size to destroy at

    /*
    protected new void Awake()
    {
        //Call Base Function to initalise
        base.Awake();

        //Place on Surface now to avoid snapping
        if (planet)
        {
            objectRB.MovePosition(planet.GetPlaceOnSurfacePos(objectRB));
        }
    }*/

    // Update is called once per frame
    private void Update()
    {
        //Shrink Over time
        if (GetComponent<Collider>().bounds.size.x >= destroySize)
        {
            transform.localScale *= 1f - (shrinkSpeed * Time.deltaTime);
        }
        else
        {
            //Destroy the object once we are under the minimum size
            Destroy(gameObject);
        }
    }
}
