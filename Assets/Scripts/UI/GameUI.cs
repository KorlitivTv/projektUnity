using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    private static Canvas runtimeOverlayCanvas;

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
            GameObject waveObject = FindSceneObject("WaveAnnouncement");
            if (waveObject != null)
            {
                waveAnnouncement = waveObject.GetComponent<TextMeshProUGUI>();
            }
        }

        if (gameOverPanel != null)
        {
            MoveToRuntimeOverlay(gameOverPanel, 10000, new Vector2(750f, 620f));
        }

        if (waveAnnouncement != null)
        {
            MoveToRuntimeOverlay(waveAnnouncement.gameObject, 11000, new Vector2(900f, 180f));
        }

        AutoAssignGameOverTexts();
    }

    private void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (waveAnnouncement != null)
        {
            waveAnnouncement.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            IsGameOver = false;
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
            GameObject waveObject = FindSceneObject("WaveAnnouncement");
            if (waveObject != null)
            {
                waveAnnouncement = waveObject.GetComponent<TextMeshProUGUI>();
                MoveToRuntimeOverlay(waveObject, 11000, new Vector2(900f, 180f));
            }
        }

        if (waveAnnouncement == null)
        {
            Debug.LogWarning("WaveAnnouncement could not be found.");
            yield break;
        }

        waveAnnouncement.text = isBossWave ? "BOSS WAVE " + wave : "WAVE " + wave;
        PrepareUiObject(waveAnnouncement.gameObject, new Vector2(900f, 180f));

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

        waveAnnouncement.gameObject.SetActive(false);
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
            if (gameOverPanel != null)
            {
                MoveToRuntimeOverlay(gameOverPanel, 10000, new Vector2(750f, 620f));
            }
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
            PrepareUiObject(gameOverPanel, new Vector2(750f, 620f));
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

    private static void MoveToRuntimeOverlay(GameObject target, int sortingOrder, Vector2 minimumSize)
    {
        Canvas overlay = GetRuntimeOverlayCanvas(sortingOrder);
        target.transform.SetParent(overlay.transform, false);
        PrepareUiObject(target, minimumSize);
    }

    private static Canvas GetRuntimeOverlayCanvas(int sortingOrder)
    {
        if (runtimeOverlayCanvas == null)
        {
            GameObject canvasObject = new GameObject("RuntimeGameOverlay");
            runtimeOverlayCanvas = canvasObject.AddComponent<Canvas>();
            runtimeOverlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            runtimeOverlayCanvas.overrideSorting = true;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();
        }

        runtimeOverlayCanvas.sortingOrder = Mathf.Max(runtimeOverlayCanvas.sortingOrder, sortingOrder);
        return runtimeOverlayCanvas;
    }

    private static void PrepareUiObject(GameObject target, Vector2 minimumSize)
    {
        target.SetActive(true);
        target.transform.SetAsLastSibling();

        CanvasGroup group = target.GetComponent<CanvasGroup>();
        if (group == null)
        {
            group = target.AddComponent<CanvasGroup>();
        }

        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;

        RectTransform rect = target.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
            rect.sizeDelta = new Vector2(
                Mathf.Max(rect.sizeDelta.x, minimumSize.x),
                Mathf.Max(rect.sizeDelta.y, minimumSize.y)
            );
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
