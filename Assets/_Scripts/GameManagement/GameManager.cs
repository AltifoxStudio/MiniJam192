using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // --- Singleton Instance ---
    public static GameManager Instance { get; private set; }

    // --- Inspector References ---
    [Header("Core Prefabs")]
    [SerializeField] private GameObject vacuumCleanerPrefab;
    [SerializeField] private GameObject dustBallPrefab;
    [SerializeField] private GameObject playerPrefab;
    
    [Header("Configuration")]
    [SerializeField] private GameConfig gameConfig;

    // --- Level State ---
    public int TotalDustBallsInLevel { get; private set; }
    public int CollectedDustBalls { get; private set; } = 0;

    private Dictionary<Transform, GameObject> _activeDustBalls = new Dictionary<Transform, GameObject>();
    private Dictionary<Transform, GameObject> _activeVacuums = new Dictionary<Transform, GameObject>();

    private void Awake()
    {
        // Singleton logic
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Initialize the level once the scene is fully loaded and all Awakes are done.
        InitLevel();
    }

    private void InitLevel()
    {
        // Find all spawner locations in the scene
        var dustBallSpawners = FindObjectsByType<DustBallsSpawners>(FindObjectsSortMode.None).Select(s => s.transform).ToArray();
        var vacuumSpawners = FindObjectsByType<VacuumSpawner>(FindObjectsSortMode.None).Select(s => s.transform).ToArray();
        var playerSpawner = FindFirstObjectByType<PlayerSpawner>().transform;

        TotalDustBallsInLevel = dustBallSpawners.Length;

        // Spawn Dust Balls
        foreach (Transform spawner in dustBallSpawners)
        {
            GameObject dustGO = Instantiate(dustBallPrefab, spawner.position, spawner.rotation);
            _activeDustBalls.Add(spawner, dustGO);
        }

        // Spawn Vacuums
        foreach (Transform spawner in vacuumSpawners)
        {
            GameObject vacuum = Instantiate(vacuumCleanerPrefab, spawner.position, spawner.rotation);
            _activeVacuums.Add(spawner, vacuum);
        }

        // Spawn Player
        Instantiate(playerPrefab, playerSpawner.position, playerSpawner.rotation);

        Debug.Log("Level Initialized.");
    }

    private void Update()
    {
        // Check for win condition: all vacuums are destroyed.
        int remainingVacuums = _activeVacuums.Values.Count(v => v != null);
        if (remainingVacuums == 0)
        {
            OnWinLevel(SceneManager.GetActiveScene().buildIndex);
            this.enabled = false; // Disable component to stop further checks.
        }
    }

    private void FixedUpdate()
    {
        // Check if any dust balls need to be respawned.
        RespawnDustBalls();
    }
    
    private void RespawnDustBalls()
    {
        // Using ToList() creates a copy, preventing modification errors during iteration.
        foreach (Transform spawner in _activeDustBalls.Keys.ToList())
        {
            if (_activeDustBalls[spawner] == null) // Check if the dust ball was destroyed
            {
                // Respawn based on a frame rate-independent chance.
                if (Random.value < gameConfig.spawnChanceEachTick * Time.deltaTime)
                {
                    GameObject newDustBall = Instantiate(dustBallPrefab, spawner.position, spawner.rotation);
                    _activeDustBalls[spawner] = newDustBall; // Update the dictionary reference
                }
            }
        }
    }

    // --- Public API ---

    public void OnDustBallCollected()
    {
        CollectedDustBalls++;
        Debug.Log($"Dust balls collected: {CollectedDustBalls}/{TotalDustBallsInLevel}");
        // Here you could update UI or check other conditions.
    }

    public void ReloadCurrentScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        UIGameManager.Instance.ResetUI();
    }

    public void OnDeath()
    {
        UIGameManager.Instance.OnDeath();
    }

    public void OnWinLevel(int levelIndex)
    {
        UIGameManager.Instance.OnWinLevel(levelIndex);
    }
}