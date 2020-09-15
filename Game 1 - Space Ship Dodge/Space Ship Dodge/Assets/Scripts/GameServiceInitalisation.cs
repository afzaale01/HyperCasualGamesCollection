using System.Collections;
using System.Collections.Generic;
using Lewis.Social.Face;
using UnityEngine;


/// <summary>
/// Class for initliasing game services (i.e music, Social Media etc)
/// </summary>
public class GameServiceInitalisation : MonoBehaviour
{
    private void Awake()
    {
        //Init Facebook
        FacebookIntergration.InitFacebook();

        //We are done initalising - destroy
        Destroy(gameObject);
    }
}
