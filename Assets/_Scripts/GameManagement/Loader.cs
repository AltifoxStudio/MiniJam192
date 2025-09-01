using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management events

public class Loader : MonoBehaviour
{
    // --- Singleton Instance ---
    public static Loader Instance { get; private set; }

    // --- Prefab References ---
    [Header("Persistent Systems (Don't Destroy On Load)")]
    [SerializeField] private GameObject uiPrefab;
    [SerializeField] private GameObject musicPrefab;
    [SerializeField] private GameObject gameLogicPrefab;  // Contains GameManager

    private void Awake()
    {
        // Singleton Pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Instantiate persistent systems ONLY ONCE
        InstantiatePersistentSystems();
    }

    // This method will be called every time a new scene finishes loading
    
    private void InstantiatePersistentSystems()
    {
        if (gameLogicPrefab != null)
        {
            Instantiate(gameLogicPrefab, transform);
        }

        if (uiPrefab != null)
        {
            Instantiate(uiPrefab, transform);
        }

        if (musicPrefab != null)
        {
            Instantiate(musicPrefab, transform);
        }
    }
}