using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    private bool isPaused;

    private void Awake()
    {
        if (pausePanel == null)
        {
            pausePanel = FindSceneObject("PausePanel");
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
        }

        isPaused = !isPaused;

        if (pausePanel != null)
        {
            if (isPaused)
            {
                ForcePanelVisible(pausePanel);
            }
            else
            {
                pausePanel.SetActive(false);
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

    private static void ForcePanelVisible(GameObject panel)
    {
        Transform current = panel.transform.parent;
        while (current != null)
        {
            current.gameObject.SetActive(true);
            current = current.parent;
        }

        panel.SetActive(true);
        panel.transform.SetAsLastSibling();

        CanvasGroup group = panel.GetComponent<CanvasGroup>();
        if (group != null)
        {
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }

        RectTransform rect = panel.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;

            if (rect.sizeDelta.x < 300f || rect.sizeDelta.y < 200f)
            {
                rect.sizeDelta = new Vector2(600f, 500f);
            }
        }

        Canvas canvas = panel.GetComponentInParent<Canvas>(true);
        if (canvas != null)
        {
            canvas.enabled = true;
            canvas.overrideSorting = true;
            canvas.sortingOrder = 5000;
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