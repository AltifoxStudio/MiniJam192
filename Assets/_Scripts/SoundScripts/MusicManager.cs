using UnityEngine;
using AltifoxStudio.AltifoxAudioManager;
using System;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    // Music Players
    public AltifoxPersistentPlayer inGameMusic;
    public string menuMusicName;
    public string inGameMusicName;

    [Header("Config")]
    public bool first = true;


    private void Start()
    {
        inGameMusic.SetForPlay(menuMusicName);
        inGameMusic.SetForPlay(inGameMusicName);
    }

    private void Update()
    {
        //Debug.Log($"Music playing: {inGameMusic.currentPlayingTrack}, is it playing ? {inGameMusic.isPlaying}");
        if (first)
        {
            inGameMusic.Play();
            first = false;
        }
    }

}