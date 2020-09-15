using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTile : WalkableTile
{
    //Event for when the player is killed
    public static event TileEvent EndTileReached;

    private new void OnTriggerEnter(Collider other)
    {
        //Call base function
        base.OnTriggerEnter(other);

        //Check that the player exists
        if (other.gameObject.GetComponent<PlayerMovement>())
        {
            //Trigger Level Completed Event
            EndTileReached?.Invoke();
        }
    }
}
