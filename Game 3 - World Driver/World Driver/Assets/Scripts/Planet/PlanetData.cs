using System.Collections;
using System.Collections.Generic;
using Lewis.MathUtils;
using UnityEngine;

/// <summary>
/// Class for shared data between planet elements
/// </summary>
public class PlanetData : MonoBehaviour
{
    //Property for the current planet size, the max of 
    //the local scale values of the planet
    public float PlanetSize
    {
        get { return MathUtils.MaxVecValue(transform.localScale); }
    }

}
