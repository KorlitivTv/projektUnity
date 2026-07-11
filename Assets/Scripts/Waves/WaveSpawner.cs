using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Wave Settings")]
    [SerializeField] private float timeBetweenSpawns = 0.75f;
    [SerializeField] private float timeBetweenWaves = 3f;
    [SerializeField] private int enemiesInFirstWave = 3;
    [SerializeField] private int enemiesAddedPerWave = 2;
    [SerializeField] private int scorePerEnemy = 10;

    private static WaveSpawner instance;
    private static int enemiesAlive;
    private static int score;

    private int currentWave;
    private bool spawningWave;

    private void Awake()
    {
        instance = this;
        enemiesAlive = 0;
        score = 0;
    }

    private void Start()
    {
        UpdateUI();
        StartCoroutine(StartNextWave());
    }

    private void Update()
    {
        if (!spawningWave && enemiesAlive <= 0)
        {
            StartCoroutine(StartNextWave());
        }
    }

    private IEnumerator StartNextWave()
    {
        spawningWave = true;
        currentWave++;

        Debug.Log("Wave " + currentWave);
        UpdateUI();

        yield return new WaitForSeconds(timeBetweenWaves);

        int enemyCount = enemiesInFirstWave + (currentWave - 1) * enemiesAddedPerWave;

        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        spawningWave = false;
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("WaveSpawner is missing Enemy Prefab.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("WaveSpawner has no Spawn Points.");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        enemiesAlive++;
        UpdateUI();
    }

    public static void EnemyDefeated()
    {
        if (instance == null)
        {
            return;
        }

        enemiesAlive = Mathf.Max(0, enemiesAlive - 1);
        score += instance.scorePerEnemy;
        instance.UpdateUI();
    }

    private void UpdateUI()
    {
        GameUI.Instance?.SetWave(currentWave);
        GameUI.Instance?.SetEnemies(enemiesAlive);
        GameUI.Instance?.SetScore(score);
    }
}
