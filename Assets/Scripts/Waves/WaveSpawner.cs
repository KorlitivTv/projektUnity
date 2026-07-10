using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float timeBetweenSpawns = 0.75f;
    [SerializeField] private float timeBetweenWaves = 3f;
    [SerializeField] private int enemiesInFirstWave = 3;
    [SerializeField] private int enemiesAddedPerWave = 2;

    private static int enemiesAlive;
    private int currentWave;
    private bool spawningWave;

    private void Start()
    {
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
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("WaveSpawner is missing enemy prefab or spawn points.");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        enemiesAlive++;
    }

    public static void EnemyDefeated()
    {
        enemiesAlive--;
        if (enemiesAlive < 0)
        {
            enemiesAlive = 0;
        }
    }
}
