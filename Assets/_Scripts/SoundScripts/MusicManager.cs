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
    public float RandomLayerChance = 0.2f;


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

        if (inGameMusic.isPlaying && UnityEngine.Random.value < RandomLayerChance * Time.deltaTime)
        {
            Debug.Log("Changing configuration");
            // Deactivate both layers first to ensure a clean state
            inGameMusic.SetLayerActive("Melody 1", false);
            inGameMusic.SetLayerActive("Melody 2", false);

            // Get a random integer: 0, 1, or 2
            int choice = UnityEngine.Random.Range(0, 3);

            switch (choice)
            {
                case 0:
                    // Outcome 1: Play only Melody 1 (1/3 chance)
                    inGameMusic.SetLayerActive("Melody 1", true);
                    break;
                case 1:
                    // Outcome 2: Play only Melody 2 (1/3 chance)
                    inGameMusic.SetLayerActive("Melody 2", true);
                    break;
                case 2:
                    // Outcome 3: Play both (1/3 chance)
                    inGameMusic.SetLayerActive("Melody 1", true);
                    inGameMusic.SetLayerActive("Melody 2", true);
                    break;
            }
        }
    }

}