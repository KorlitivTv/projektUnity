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
        SetPanelVisible(false);
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

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        SetPanelVisible(isPaused);
    }

    public void Resume()
    {
        if (GameUI.IsGameOver)
        {
            return;
        }

        isPaused = false;
        Time.timeScale = 1f;
        SetPanelVisible(false);
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

    private void SetPanelVisible(bool visible)
    {
        if (pausePanel == null)
        {
            pausePanel = FindSceneObject("PausePanel");
        }

        if (pausePanel == null)
        {
            Debug.LogError("PauseMenuController could not find PausePanel.");
            return;
        }

        EnsureParentsActive(pausePanel.transform.parent);
        pausePanel.SetActive(visible);

        CanvasGroup group = pausePanel.GetComponent<CanvasGroup>();
        if (group != null)
        {
            group.alpha = 1f;
            group.interactable = visible;
            group.blocksRaycasts = visible;
        }

        if (visible)
        {
            pausePanel.transform.SetAsLastSibling();
        }
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
