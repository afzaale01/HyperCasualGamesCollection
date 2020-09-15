using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MusicTools : MonoBehaviour
{
    /// <summary>
    /// Force to options to change the music to a random track
    /// </summary>
    [MenuItem("Game IO/Music/Force Track Change")]
    public static void ChangeMusicToRandomTrack()
    {
        //Find the music player
        MusicPlayer player = FindObjectOfType<MusicPlayer>();

        if (player)
        {
            player.PlayRandomTrack();
        }
    }
}
