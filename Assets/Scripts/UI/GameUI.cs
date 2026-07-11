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
            MoveToRuntimeOverlay(gameOverPanel, 10000, new Vector2(700f, 620f));
            AutoAssignGameOverTexts();
            LayoutGameOverPanel();
        }

        if (waveAnnouncement != null)
        {
            MoveToRuntimeOverlay(waveAnnouncement.gameObject, 11000, new Vector2(900f, 180f));
            LayoutWaveAnnouncement();
        }
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
                LayoutWaveAnnouncement();
            }
        }

        if (waveAnnouncement == null)
        {
            Debug.LogWarning("WaveAnnouncement could not be found.");
            yield break;
        }

        waveAnnouncement.text = isBossWave ? "BOSS WAVE " + wave : "WAVE " + wave;
        PrepareUiObject(waveAnnouncement.gameObject, new Vector2(900f, 180f), exactSize: true);
        LayoutWaveAnnouncement();

        Color color = isBossWave ? new Color(1f, 0.35f, 0.2f, 1f) : Color.white;
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
                MoveToRuntimeOverlay(gameOverPanel, 10000, new Vector2(700f, 620f));
                AutoAssignGameOverTexts();
            }
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
            PrepareUiObject(gameOverPanel, new Vector2(700f, 620f), exactSize: true);
            LayoutGameOverPanel();
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

    private void LayoutGameOverPanel()
    {
        if (gameOverPanel == null)
        {
            return;
        }

        RectTransform panelRect = gameOverPanel.GetComponent<RectTransform>();
        if (panelRect != null)
        {
            panelRect.sizeDelta = new Vector2(700f, 620f);
            panelRect.anchoredPosition = Vector2.zero;
        }

        Image background = gameOverPanel.GetComponent<Image>();
        if (background != null)
        {
            background.sprite = null;
            background.type = Image.Type.Simple;
            background.color = new Color(0.035f, 0.035f, 0.055f, 0.96f);
        }

        LayoutText("Game Over", new Vector2(0f, 220f), new Vector2(620f, 90f), 58f, FontStyles.Bold);
        LayoutText("FinalScoreText", new Vector2(0f, 90f), new Vector2(560f, 55f), 32f, FontStyles.Normal);
        LayoutText("BestScoreText", new Vector2(0f, 30f), new Vector2(560f, 55f), 32f, FontStyles.Normal);
        LayoutText("WaveReachedText", new Vector2(0f, -30f), new Vector2(560f, 55f), 30f, FontStyles.Normal);

        LayoutButton("Restart", new Vector2(0f, -145f), new Vector2(320f, 62f));
        LayoutButton("Main Menu", new Vector2(0f, -225f), new Vector2(320f, 62f));
    }

    private void LayoutWaveAnnouncement()
    {
        if (waveAnnouncement == null)
        {
            return;
        }

        RectTransform rect = waveAnnouncement.rectTransform;
        rect.sizeDelta = new Vector2(900f, 180f);
        rect.anchoredPosition = Vector2.zero;

        waveAnnouncement.fontSize = 64f;
        waveAnnouncement.fontStyle = FontStyles.Bold;
        waveAnnouncement.alignment = TextAlignmentOptions.Center;
        waveAnnouncement.enableAutoSizing = false;
        waveAnnouncement.raycastTarget = false;
    }

    private void LayoutText(string objectName, Vector2 position, Vector2 size, float fontSize, FontStyles style)
    {
        Transform child = FindChildRecursive(gameOverPanel.transform, objectName);
        if (child == null)
        {
            return;
        }

        RectTransform rect = child.GetComponent<RectTransform>();
        if (rect != null)
        {
            SetCenteredRect(rect, position, size);
        }

        TextMeshProUGUI text = child.GetComponent<TextMeshProUGUI>();
        if (text != null)
        {
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.alignment = TextAlignmentOptions.Center;
            text.enableAutoSizing = false;
            text.color = Color.white;
        }
    }

    private void LayoutButton(string objectName, Vector2 position, Vector2 size)
    {
        Transform child = FindChildRecursive(gameOverPanel.transform, objectName);
        if (child == null)
        {
            return;
        }

        RectTransform rect = child.GetComponent<RectTransform>();
        if (rect != null)
        {
            SetCenteredRect(rect, position, size);
        }

        Image image = child.GetComponent<Image>();
        if (image != null)
        {
            image.color = new Color(0.18f, 0.18f, 0.24f, 1f);
        }

        TextMeshProUGUI label = child.GetComponentInChildren<TextMeshProUGUI>(true);
        if (label != null)
        {
            label.fontSize = 28f;
            label.alignment = TextAlignmentOptions.Center;
            label.enableAutoSizing = false;
            label.color = Color.white;
        }
    }

    private static void SetCenteredRect(RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
    }

    private static Transform FindChildRecursive(Transform parent, string objectName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == objectName)
            {
                return child;
            }

            Transform result = FindChildRecursive(child, objectName);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    private static void MoveToRuntimeOverlay(GameObject target, int sortingOrder, Vector2 size)
    {
        Canvas overlay = GetRuntimeOverlayCanvas(sortingOrder);
        target.transform.SetParent(overlay.transform, false);
        PrepareUiObject(target, size, exactSize: true);
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

    private static void PrepareUiObject(GameObject target, Vector2 size, bool exactSize)
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
            rect.sizeDelta = exactSize
                ? size
                : new Vector2(Mathf.Max(rect.sizeDelta.x, size.x), Mathf.Max(rect.sizeDelta.y, size.y));
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
