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

    [Header("Initial Level Prefabs")]
    [SerializeField] private GameObject environmentAnd3DPrefab;
    [SerializeField] private GameObject gameDesignPrefab; // Contains spawners
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

    private void OnEnable()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent errors and memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // This method will be called every time a new scene finishes loading
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Now, we load the level content every time a scene loads.
        LoadLevelContent();
    }
    
    private void InstantiatePersistentSystems()
    {
        if (uiPrefab != null)
        {
            Instantiate(uiPrefab, transform);
        }

        if (musicPrefab != null)
        {
            Instantiate(musicPrefab, transform);
        }
    }

    public void LoadLevelContent()
    {
        Debug.Log("Loading level content for scene: " + SceneManager.GetActiveScene().name);

        // Instantiate all the necessary parts of the level.
        if (environmentAnd3DPrefab == null || gameDesignPrefab == null || gameLogicPrefab == null)
        {
            Debug.LogError("A core level prefab is missing from the Loader inspector!");
            return;
        }

        Instantiate(environmentAnd3DPrefab, Vector3.zero, Quaternion.identity);
        Instantiate(gameDesignPrefab, Vector3.zero, Quaternion.identity);
        Instantiate(gameLogicPrefab, Vector3.zero, Quaternion.identity);
    }
}