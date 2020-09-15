using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object that is used to store infomation about
/// cars.
/// Stores Name, Stats, Model etc.
/// </summary>
[CreateAssetMenu(fileName = "New Card", menuName = "Custom/Car")]
public class CarInfo : ScriptableObject
{
    //Display Name of the car
    public new string name;

    //Model of the car
    public GameObject model;

    //Stats about speed and handiling
    //These are mutipliers on the base car stats
    public float speedMutiplier = 1f;
    public float handilingMutiplier = 1f;
}
