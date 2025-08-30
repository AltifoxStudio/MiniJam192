using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    // 1. Public static instance
    public static GameManager Instance { get; private set; }

    public GameObject vaccumCleanerPrefab;
    public GameObject DustBallPrefab;

    private Transform vacuumSpawnPoint;
    private Transform[] dustBallsSpawnPoints;

    public int totalDustBallsInLevel;
    public int collectedDustBalls = 0;

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
            Instantiate(DustBallPrefab, dustSpawner.position, dustSpawner.rotation);
        }
        Instantiate(vaccumCleanerPrefab, vacuumSpawnPoint.position, vacuumSpawnPoint.rotation);
    }

    // You can add public methods here to be called from other scripts
    public void OnDustBallCollected()
    {
        collectedDustBalls++;
        // Check for win condition, update UI, etc.
        Debug.Log($"Dust balls collected: {collectedDustBalls}/{totalDustBallsInLevel}");
    }
}