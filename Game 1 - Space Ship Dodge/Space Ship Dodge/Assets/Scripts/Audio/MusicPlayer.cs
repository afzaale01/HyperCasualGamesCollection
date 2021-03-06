using System.Collections;
using System.Collections.Generic;
using Lewis.GameOptions;
using UnityEngine;

/// <summary>
/// Music Player that handles all of the playing of music throught the game
/// </summary>
public class MusicPlayer : MonoBehaviour
{
    //Instance of the music player - only one should exist
    private static MusicPlayer instance;

    //Location (relative to resoruces folder) to load audio tracks from
    private const string musicLoadPath = "Music/";

    //List of all of the audio tracks
    private AudioClip[] musicTracks;
    private AudioClip currentTrack; //Current Playing Track

    //Audio sources to use to fade between tracks, generated by this script
    private AudioSource[] musicPlayers;
    private int activeMusicPlayerIndex;

    [SerializeField] private float trackFadeStartTime = 5f; //Time (seconds) from the end of an audio track to fade out/in the old/new track
    [SerializeField] private float audioFadeSpeed = 0.05f; //Amount to change the audio volume per interval
    [SerializeField] private float audioFadeInterval = 0.01f; //Time (seconds) between audio fade updates

    //If we are currently fading between tracks
    private bool transitioningTrack = false;

    //Target Volume to reach when we are fading in music tracks
    private float targetMusicVolume = 1f;

    // Start is called before the first frame update
    void Start()
    {
        //Check and instance doesn't already exist
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        //Load music tracks
        LoadMusicTracks();

        //Generate 2 audio soruces so that we can toggle between them for
        //fading tracks
        musicPlayers = new AudioSource[2];
        musicPlayers[0] = gameObject.AddComponent<AudioSource>();
        musicPlayers[1] = gameObject.AddComponent<AudioSource>();
        activeMusicPlayerIndex = 0;

        //Update Music Volume
        UpdateMusicVolume();

        //Play the first music track
        PlayRandomTrack();
    }

    // Update is called once per frame
    void Update()
    {
        //Check what time we are at in the current track, if we are under
        //the limit then change the track
        if (currentTrack && !transitioningTrack)
        {
            float currentTrackTimeRemaining = currentTrack.length - musicPlayers[activeMusicPlayerIndex].time;
            if (currentTrackTimeRemaining < trackFadeStartTime)
            {
                PlayRandomTrack();
            }
        }
    }

    /// <summary>
    /// Play a given music track
    /// </summary>
    /// <param name="nextTrack">Track to play</param>
    private void PlayMusicTrack(AudioClip nextTrack)
    {
        if (currentTrack == null)
        {
            //If no track is playing then start this track straigt away
            musicPlayers[activeMusicPlayerIndex].clip = nextTrack;
            musicPlayers[activeMusicPlayerIndex].volume = targetMusicVolume;
            musicPlayers[activeMusicPlayerIndex].Play();
            currentTrack = nextTrack;
        }
        else
        {
            //Otherwise fade in the the new track and out the old one
            StartCoroutine(TransitionToMusicTrack(nextTrack));
        }
    }

    /// <summary>
    /// Plays a random music track
    /// </summary>
    public void PlayRandomTrack()
    {
        if (musicTracks.Length > 0)
        {
            //Find a random track to play
            int randomTrackIndex = Random.Range(0, musicTracks.Length);
            AudioClip trackToPlay = musicTracks[randomTrackIndex];

            //Call play track function with this selected track
            PlayMusicTrack(trackToPlay);
        }
    }

    /// <summary>
    /// Load all of the music tracks to play
    /// </summary>
    private void LoadMusicTracks()
    {
        //Load all of the music tracks
        musicTracks = Resources.LoadAll<AudioClip>(musicLoadPath);

        //Check that audio tracks are loaded
        if (musicTracks.Length > 0)
        {
            Debug.Log($"[SUCCESS] Loaded {musicTracks.Length} Music Tracks");
        }
        else
        {
            Debug.LogWarning("[FAILED] Did not load any music tracks");
        }
    }

    /// <summary>
    /// Transision between the current music track and a new music track
    /// </summary>
    /// <param name="nextTrack">New music track to play</param>
    /// <returns></returns>
    private IEnumerator TransitionToMusicTrack(AudioClip nextTrack)
    {
        transitioningTrack = true;

        //Get the the unused music player to fade in song
        int nextTrackPlayerIndex = activeMusicPlayerIndex == 0 ? 1 : 0; //Toggle between 0 and 1
        AudioSource nextTrackPlayer = musicPlayers[nextTrackPlayerIndex];
        AudioSource prevTrackPlayer = musicPlayers[activeMusicPlayerIndex];

        //Set our new audio player to load this track and play it at 0 volume
        nextTrackPlayer.clip = nextTrack;
        nextTrackPlayer.volume = 0f;
        nextTrackPlayer.Play();

        //Loop until the new audio's volume is at our music volume level
        while (nextTrackPlayer.volume < targetMusicVolume)
        {
            //Increase the new music volume and decrease the old music volume
            //Mutiply by targetMusicVolume so we fade at the same rate regardless of the
            //music volume
            nextTrackPlayer.volume += audioFadeSpeed * targetMusicVolume;
            prevTrackPlayer.volume -= audioFadeSpeed * targetMusicVolume;

            //Wait 
            yield return new WaitForSecondsRealtime(audioFadeInterval);
        }

        //Now our new track is playing on our new audio soruce transision the varaibles
        //so that everything is updated
        prevTrackPlayer.volume = 0f; //Just to be sure
        prevTrackPlayer.Stop();

        currentTrack = nextTrack;
        activeMusicPlayerIndex = nextTrackPlayerIndex;

        transitioningTrack = false;
    }

    /// <summary>
    /// Update the music volume of future and currently
    /// playing tracks.
    /// Called by event when options are updated
    /// </summary>
    private void UpdateMusicVolume()
    {
        float newVolume = GameOptionsManager.GetCurrentGameOptions().musicVolume;

        //Set active player volume
        if (musicPlayers[activeMusicPlayerIndex])
        {
            musicPlayers[activeMusicPlayerIndex].volume = newVolume;
        }

        //Set target volume for when we are fading in songs
        targetMusicVolume = newVolume;
    }

    #region Event Sub/UnSub

    private void OnEnable()
    {
        GameOptionsManager.GameOptionsUpdated += UpdateMusicVolume;
    }

    private void OnDisable()
    {
        GameOptionsManager.GameOptionsUpdated -= UpdateMusicVolume;
    }

    #endregion
}
