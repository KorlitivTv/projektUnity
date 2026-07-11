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
        if (pausePanel == null)
        {
            pausePanel = FindSceneObject("PausePanel");
        }

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
            pausePanel.transform.SetAsLastSibling();
        }
    }

    public void Resume()
    {
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
