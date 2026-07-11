using System.Collections;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance { get; private set; }

    [Header("HUD Text Labels")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI enemiesText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private TextMeshProUGUI waveReachedText;

    [Header("Wave Announcement")]
    [SerializeField] private TextMeshProUGUI waveAnnouncement;
    [SerializeField] private float announcementHoldTime = 1.2f;
    [SerializeField] private float announcementFadeTime = 0.6f;

    private const string BestScoreKey = "BestScore";

    private void Awake()
    {
        Instance = this;

        if (gameOverPanel == null)
        {
            gameOverPanel = FindSceneObject("GameOverPanel");
        }

        AutoAssignGameOverTexts();
    }

    private void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("GameUI could not find GameOverPanel.");
        }

        if (waveAnnouncement != null)
        {
            waveAnnouncement.gameObject.SetActive(false);
        }
    }

    public void SetHealth(int currentHealth, int maxHealth)
    {
        if (healthText != null)
        {
            healthText.text = "HP: " + currentHealth + " / " + maxHealth;
        }
    }

    public void SetWave(int wave)
    {
        if (waveText != null)
        {
            waveText.text = "Wave: " + wave;
        }
    }

    public void SetEnemies(int enemies)
    {
        if (enemiesText != null)
        {
            enemiesText.text = "Enemies: " + enemies;
        }
    }

    public void SetScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    public IEnumerator ShowWaveAnnouncement(int wave, bool isBossWave)
    {
        if (waveAnnouncement == null)
        {
            yield break;
        }

        waveAnnouncement.text = isBossWave
            ? "BOSS WAVE " + wave
            : "WAVE " + wave;

        Color color = waveAnnouncement.color;
        color.a = 1f;
        waveAnnouncement.color = color;
        waveAnnouncement.gameObject.SetActive(true);

        yield return new WaitForSeconds(announcementHoldTime);

        float elapsed = 0f;
        float fadeTime = Mathf.Max(0.01f, announcementFadeTime);

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            waveAnnouncement.color = color;
            yield return null;
        }

        waveAnnouncement.gameObject.SetActive(false);
    }

    public void ShowGameOver()
    {
        if (gameOverPanel == null)
        {
            gameOverPanel = FindSceneObject("GameOverPanel");
            AutoAssignGameOverTexts();
        }

        int finalScore = WaveSpawner.CurrentScore;
        int waveReached = WaveSpawner.CurrentWave;
        int bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);

        if (finalScore > bestScore)
        {
            bestScore = finalScore;
            PlayerPrefs.SetInt(BestScoreKey, bestScore);
            PlayerPrefs.Save();
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + finalScore;
        }

        if (bestScoreText != null)
        {
            bestScoreText.text = "Best Score: " + bestScore;
        }

        if (waveReachedText != null)
        {
            string waveWord = waveReached == 1 ? "wave" : "waves";
            waveReachedText.text = "You survived " + waveReached + " " + waveWord;
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            gameOverPanel.transform.SetAsLastSibling();
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogError("Game Over triggered, but GameOverPanel could not be found.");
        }
    }

    private void AutoAssignGameOverTexts()
    {
        if (gameOverPanel == null)
        {
            return;
        }

        TextMeshProUGUI[] labels = gameOverPanel.GetComponentsInChildren<TextMeshProUGUI>(true);

        foreach (TextMeshProUGUI label in labels)
        {
            if (finalScoreText == null && label.name == "FinalScoreText")
            {
                finalScoreText = label;
            }
            else if (bestScoreText == null && label.name == "BestScoreText")
            {
                bestScoreText = label;
            }
            else if (waveReachedText == null && label.name == "WaveReachedText")
            {
                waveReachedText = label;
            }
        }
    }

    private static GameObject FindSceneObject(string objectName)
    {
        Transform[] transforms = Object.FindObjectsByType<Transform>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (Transform candidate in transforms)
        {
            if (candidate.gameObject.scene.isLoaded && candidate.name == objectName)
            {
                return candidate.gameObject;
            }
        }

        return null;
    }
}
