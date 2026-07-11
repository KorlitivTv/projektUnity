using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Wave Settings")]
    [SerializeField] private float timeBetweenSpawns = 0.75f;
    [SerializeField] private float timeBetweenWaves = 3f;
    [SerializeField] private int enemiesInFirstWave = 2;
    [SerializeField] private int enemiesAddedPerWave = 1;
    [SerializeField] private int scorePerEnemy = 10;

    [Header("Boss Settings")]
    [SerializeField] private int bossEveryNWaves = 5;
    [SerializeField] private int bossesPerBossWave = 1;

    private static WaveSpawner instance;
    private static int enemiesAlive;
    private static int score;

    private int currentWave;
    private bool spawningWave;

    public static int CurrentScore => score;
    public static int CurrentWave => instance != null ? instance.currentWave : 0;

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

        bool isBossWave = bossEveryNWaves > 0 && currentWave % bossEveryNWaves == 0;

        Debug.Log(isBossWave ? "Boss Wave " + currentWave : "Wave " + currentWave);
        UpdateUI();

        if (GameUI.Instance != null)
        {
            yield return StartCoroutine(GameUI.Instance.ShowWaveAnnouncement(currentWave, isBossWave));
        }

        yield return new WaitForSeconds(timeBetweenWaves);

        if (isBossWave)
        {
            int bossCount = Mathf.Max(1, bossesPerBossWave);

            for (int i = 0; i < bossCount; i++)
            {
                SpawnPrefab(bossPrefab, "Boss Prefab");
                yield return new WaitForSeconds(timeBetweenSpawns);
            }
        }
        else
        {
            int enemyCount = enemiesInFirstWave + (currentWave - 1) * enemiesAddedPerWave;

            for (int i = 0; i < enemyCount; i++)
            {
                SpawnPrefab(enemyPrefab, "Enemy Prefab");
                yield return new WaitForSeconds(timeBetweenSpawns);
            }
        }

        spawningWave = false;
    }

    private void SpawnPrefab(GameObject prefab, string prefabName)
    {
        if (prefab == null)
        {
            Debug.LogWarning("WaveSpawner is missing " + prefabName + ".");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("WaveSpawner has no Spawn Points.");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(prefab, spawnPoint.position, Quaternion.identity);

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
