using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Automatic Spawn Around Camera")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float spawnDistanceOutsideView = 2f;
    [SerializeField] private int maxSpawnTries = 20;
    [SerializeField] private LayerMask blockedSpawnLayers;

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

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
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
            Debug.LogWarning("WaveSpawner is missing enemy prefab.");
            return;
        }

        Vector2 spawnPosition = GetSpawnPositionOutsideCamera();
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        enemiesAlive++;
        UpdateUI();
    }

    private Vector2 GetSpawnPositionOutsideCamera()
    {
        if (targetCamera == null)
        {
            return transform.position;
        }

        float cameraHeight = targetCamera.orthographicSize;
        float cameraWidth = cameraHeight * targetCamera.aspect;
        Vector2 cameraPosition = targetCamera.transform.position;

        for (int i = 0; i < maxSpawnTries; i++)
        {
            int side = Random.Range(0, 4);
            Vector2 spawnPosition = cameraPosition;

            switch (side)
            {
                case 0:
                    spawnPosition.x += Random.Range(-cameraWidth, cameraWidth);
                    spawnPosition.y += cameraHeight + spawnDistanceOutsideView;
                    break;
                case 1:
                    spawnPosition.x += Random.Range(-cameraWidth, cameraWidth);
                    spawnPosition.y -= cameraHeight + spawnDistanceOutsideView;
                    break;
                case 2:
                    spawnPosition.x -= cameraWidth + spawnDistanceOutsideView;
                    spawnPosition.y += Random.Range(-cameraHeight, cameraHeight);
                    break;
                default:
                    spawnPosition.x += cameraWidth + spawnDistanceOutsideView;
                    spawnPosition.y += Random.Range(-cameraHeight, cameraHeight);
                    break;
            }

            bool blocked = Physics2D.OverlapCircle(spawnPosition, 0.3f, blockedSpawnLayers);
            if (!blocked)
            {
                return spawnPosition;
            }
        }

        return cameraPosition + Random.insideUnitCircle.normalized * (cameraHeight + spawnDistanceOutsideView);
    }

    public static void EnemyDefeated()
    {
        enemiesAlive--;
        if (enemiesAlive < 0)
        {
            enemiesAlive = 0;
        }

        score += instance != null ? instance.scorePerEnemy : 10;
        instance?.UpdateUI();
    }

    private void UpdateUI()
    {
        GameUI.Instance?.SetWave(currentWave);
        GameUI.Instance?.SetEnemies(enemiesAlive);
        GameUI.Instance?.SetScore(score);
    }
}
