using System.Collections;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance { get; private set; }
    public static bool IsGameOver { get; private set; }

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
        IsGameOver = false;

        if (gameOverPanel == null)
        {
            gameOverPanel = FindSceneObject("GameOverPanel");
        }

        if (waveAnnouncement == null)
        {
            GameObject announcementObject = FindSceneObject("WaveAnnouncement");
            if (announcementObject != null)
            {
                waveAnnouncement = announcementObject.GetComponent<TextMeshProUGUI>();
            }
        }

        AutoAssignGameOverTexts();
    }

    private void Start()
    {
        SetUiObjectVisible(gameOverPanel, false);
        SetTextVisible(waveAnnouncement, false);
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
            GameObject announcementObject = FindSceneObject("WaveAnnouncement");
            if (announcementObject != null)
            {
                waveAnnouncement = announcementObject.GetComponent<TextMeshProUGUI>();
            }
        }

        if (waveAnnouncement == null)
        {
            Debug.LogError("GameUI could not find WaveAnnouncement.");
            yield break;
        }

        waveAnnouncement.text = isBossWave
            ? "BOSS WAVE " + wave
            : "WAVE " + wave;

        EnsureParentsActive(waveAnnouncement.transform.parent);
        waveAnnouncement.transform.SetAsLastSibling();
        SetTextVisible(waveAnnouncement, true);

        Color color = waveAnnouncement.color;
        color.a = 1f;
        waveAnnouncement.color = color;

        yield return new WaitForSecondsRealtime(announcementHoldTime);

        float elapsed = 0f;
        float fadeTime = Mathf.Max(0.01f, announcementFadeTime);

        while (elapsed < fadeTime)
        {
            elapsed += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            waveAnnouncement.color = color;
            yield return null;
        }

        SetTextVisible(waveAnnouncement, false);
    }

    public void ShowGameOver()
    {
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;

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
            EnsureParentsActive(gameOverPanel.transform.parent);
            SetUiObjectVisible(gameOverPanel, true);
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

    private static void SetUiObjectVisible(GameObject target, bool visible)
    {
        if (target == null)
        {
            return;
        }

        target.SetActive(visible);

        CanvasGroup group = target.GetComponent<CanvasGroup>();
        if (group != null)
        {
            group.alpha = 1f;
            group.interactable = visible;
            group.blocksRaycasts = visible;
        }
    }

    private static void SetTextVisible(TextMeshProUGUI target, bool visible)
    {
        if (target == null)
        {
            return;
        }

        target.gameObject.SetActive(visible);
        Color color = target.color;
        color.a = visible ? 1f : 0f;
        target.color = color;
    }

    private static void EnsureParentsActive(Transform parent)
    {
        while (parent != null)
        {
            if (!parent.gameObject.activeSelf)
            {
                parent.gameObject.SetActive(true);
            }

            parent = parent.parent;
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
