using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkableTile : MonoBehaviour
{
    public delegate void TileEvent();

    //If this tile has an object within it
    public bool IsOccupied
    {
        get {
            return (bool)(Occupier);
        }
    }
    public GameObject Occupier { get; private set; } = null;

    //Object that is reserving this tile
    private GameObject reserver = null;

    /// <summary>
    /// Attempt to reserve this tile.
    /// Only reserved if another object has not reserved this space
    /// </summary>
    /// <returns>If space was reserved by the calling object</returns>
    public bool AttemptToReserve(GameObject reservingObject)
    {
        //Only allow reservation if nothing else has
        if (reserver == null)
        {
            reserver = reservingObject;
            return true;
        }

        return false;
    }



    protected void OnTriggerEnter(Collider other)
    {
        //Set occuping object
        Occupier = other.gameObject;

        //When an object enters set reserved to null as we can't have
        //a reserved cell that has a object in
        reserver = null;
    }

    protected void OnTriggerExit(Collider other)
    {
        //Removing occuping object
        Occupier = null;
    }


}
