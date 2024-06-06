using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnSystem : MonoBehaviour
{
    public Transform[] spawnPoints; // Array of spawn points
    public GameObject Enemy; // Enemy prefab to spawn
    public int spawnLimit; // Limit for the number of enemies to spawn
    private int spawnCount = 0; // Counter for spawned enemies
    private int enemiesKilled = 0; // Counter for killed enemies
    public int enemiesToKill; // Number of enemies to kill to complete the level
    int randomSpawnPoint;

    void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }

        if (Enemy == null)
        {
            Debug.LogError("Enemy prefab is not assigned in the Inspector.");
            return;
        }

        // Set spawn limit and enemies to kill based on the level
        if (SceneManager.GetActiveScene().name == "Level2")
        {
            spawnLimit = 20;
            enemiesToKill = 20;
        }
        else
        {
            spawnLimit = 10;
            enemiesToKill = 10;
        }

        Debug.Log("Spawn points assigned: " + spawnPoints.Length);
        Debug.Log("Enemy prefab assigned: " + Enemy.name);

        // Start the repeating spawn process
        InvokeRepeating("SpawnEnemy", 0, 1.5f);
    }

    void SpawnEnemy()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Spawn points array is null or empty. Cannot spawn enemies.");
            return;
        }

        if (Enemy == null)
        {
            Debug.LogError("Enemy prefab is null. Cannot spawn enemies.");
            return;
        }

        // Check if we have reached the spawn limit
        if (spawnCount >= spawnLimit)
        {
            Debug.Log("Reached spawn limit. Stopping spawn process.");
            CancelInvoke("SpawnEnemy");
            return;
        }

        // Choose a random spawn point
        randomSpawnPoint = Random.Range(0, spawnPoints.Length);

        if (spawnPoints[randomSpawnPoint] == null)
        {
            Debug.LogError("Spawn point at index " + randomSpawnPoint + " is null. Skipping spawn.");
            return;
        }

        // Log spawning information
        Debug.Log("Spawning enemy at spawn point index: " + randomSpawnPoint);

        // Instantiate the enemy at the chosen spawn point
        Instantiate(Enemy, spawnPoints[randomSpawnPoint].position, Quaternion.identity);

        // Increment the spawn counter
        spawnCount++;
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
        Debug.Log("Enemies killed: " + enemiesKilled);

        // Check if all required enemies are killed to complete the level
        if (enemiesKilled >= enemiesToKill)
        {
            LoadNextLevel();
        }
    }

    private void LoadNextLevel()
    {
        Debug.Log("Loading next level...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name == "Level2" ? "Level3" : "Level2"); // Replace with your actual level names
    }

    void Update()
    {
        // Update is not used in this script but can be used for other logic if needed
    }
}
