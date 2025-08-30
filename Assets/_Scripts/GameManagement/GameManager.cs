using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // 1. Public static instance
    public static GameManager Instance { get; private set; }

    public GameObject vaccumCleanerPrefab;
    public GameObject DustBallPrefab;
    public GameConfig gameConfig;

    private Transform vacuumSpawnPoint;
    private Transform[] dustBallsSpawnPoints;

    public int totalDustBallsInLevel;
    public int collectedDustBalls = 0;

    private Dictionary<Transform, GameObject> ActiveDustBalls = new Dictionary<Transform, GameObject>();

    private void Awake()
    {
        // 2. Singleton logic
        if (Instance != null && Instance != this)
        {
            // If another instance exists, destroy this one.
            Destroy(this.gameObject);
        }
        else
        {
            // Otherwise, set this as the instance.
            Instance = this;
            // Optional: if you want the GameManager to persist across scenes
            // DontDestroyOnLoad(this.gameObject);
        }

        // Your original Awake logic
        dustBallsSpawnPoints = FindObjectsByType<DustBallsSpawners>(FindObjectsSortMode.None)
                                    .Select(dbSpawner => dbSpawner.transform)
                                    .ToArray();
        totalDustBallsInLevel = dustBallsSpawnPoints.Length;
        vacuumSpawnPoint = FindFirstObjectByType<VacuumSpawner>().transform;
    }

    private void Start()
    {
        foreach (Transform dustSpawner in dustBallsSpawnPoints)
        {
            GameObject dustGO = Instantiate(DustBallPrefab, dustSpawner.position, dustSpawner.rotation);
            ActiveDustBalls.Add(dustSpawner, dustGO);
        }
        Instantiate(vaccumCleanerPrefab, vacuumSpawnPoint.position, vacuumSpawnPoint.rotation);
    }

    private void FixedUpdate() {
        List<Transform> keys = ActiveDustBalls.Keys.ToList();
        foreach (Transform dustSpawner in keys)
        {
            // Check if the GameObject for this spawner has been destroyed (is null)
            if (ActiveDustBalls[dustSpawner] == null)
            {
                // We use Random.value which returns a float between 0.0 and 1.0.
                // Multiplying by Time.deltaTime makes the probability independent of the frame rate.
                if (Random.value < gameConfig.spawnChanceEachTick * Time.deltaTime)
                {
                    // If the random check passes, instantiate a new dust ball
                    GameObject newDustBall = Instantiate(DustBallPrefab, dustSpawner.position, dustSpawner.rotation);

                    // Update the dictionary with the reference to the newly created GameObject
                    ActiveDustBalls[dustSpawner] = newDustBall;
                }
            }
        }
    }

    public void ReloadCurrentScene()
    {
        // Get the index of the currently active scene.
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Reload the scene using its index.
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void OnDeath()
    {
        UIGameManager.Instance.OnDeath();
    }

    public void OnWinLevel(int LevelIndex)
    {
        UIGameManager.Instance.OnWinLevel(LevelIndex);
    }

    // You can add public methods here to be called from other scripts
    public void OnDustBallCollected()
    {
        collectedDustBalls++;
        // Check for win condition, update UI, etc.
        Debug.Log($"Dust balls collected: {collectedDustBalls}/{totalDustBallsInLevel}");
    }
}