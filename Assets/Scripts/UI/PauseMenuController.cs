using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    private bool isPaused;
    private static Canvas runtimeOverlayCanvas;

    private void Awake()
    {
        if (pausePanel == null)
        {
            pausePanel = FindSceneObject("PausePanel");
        }

        if (pausePanel != null)
        {
            MoveToRuntimeOverlay(pausePanel, 9000);
        }
    }

    private void Start()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        else
        {
            Debug.LogError("PauseMenuController could not find PausePanel.");
        }
    }

    private void Update()
    {
        if (GameUI.IsGameOver)
        {
            return;
        }

        bool escapePressed = Input.GetKeyDown(KeyCode.Escape);

#if ENABLE_INPUT_SYSTEM
        escapePressed |= Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
#endif

        if (escapePressed)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (GameUI.IsGameOver)
        {
            return;
        }

        if (pausePanel == null)
        {
            pausePanel = FindSceneObject("PausePanel");
            if (pausePanel != null)
            {
                MoveToRuntimeOverlay(pausePanel, 9000);
            }
        }

        isPaused = !isPaused;

        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
            if (isPaused)
            {
                PreparePanel(pausePanel, new Vector2(650f, 500f));
            }
        }

        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void Resume()
    {
        if (GameUI.IsGameOver)
        {
            return;
        }

        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private static void MoveToRuntimeOverlay(GameObject target, int sortingOrder)
    {
        Canvas overlay = GetRuntimeOverlayCanvas(sortingOrder);
        target.transform.SetParent(overlay.transform, false);
        PreparePanel(target, new Vector2(650f, 500f));
    }

    private static Canvas GetRuntimeOverlayCanvas(int sortingOrder)
    {
        if (runtimeOverlayCanvas == null)
        {
            GameObject canvasObject = new GameObject("RuntimeMenuOverlay");
            runtimeOverlayCanvas = canvasObject.AddComponent<Canvas>();
            runtimeOverlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            runtimeOverlayCanvas.overrideSorting = true;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();
        }

        runtimeOverlayCanvas.sortingOrder = sortingOrder;
        return runtimeOverlayCanvas;
    }

    private static void PreparePanel(GameObject panel, Vector2 minimumSize)
    {
        panel.SetActive(true);
        panel.transform.SetAsLastSibling();

        CanvasGroup group = panel.GetComponent<CanvasGroup>();
        if (group == null)
        {
            group = panel.AddComponent<CanvasGroup>();
        }

        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;

        RectTransform rect = panel.GetComponent<RectTransform>();
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
