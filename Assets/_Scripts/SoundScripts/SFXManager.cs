using UnityEngine;
using AltifoxStudio.AltifoxAudioManager;

public class SFXManager : MonoBehaviour
{
    // The static instance of the SFXManager, accessible from anywhere.
    public static SFXManager Instance { get; private set; }

    public AltifoxOneShotPlayer jumpSFX;
    public AltifoxOneShotPlayer collectDustSFX;

    public AltifoxOneShotPlayer deathOfAVacuum;

    private void Awake()
    {
        // This is the core singleton logic.
        // If there is no instance yet, this object becomes the instance.
        if (Instance == null)
        {
            Instance = this;
            // Optional: Prevents the manager from being destroyed when a new scene loads.
        }
        // If an instance already exists, destroy this duplicate object.
        else
        {
            Destroy(gameObject);
        }
    }
}