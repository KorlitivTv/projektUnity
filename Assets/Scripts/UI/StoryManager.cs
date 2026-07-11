using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class StoryManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";
    [SerializeField] private float inputDelay = 0.5f;

    private float sceneStartTime;
    private bool loadingGame;

    private void Start()
    {
        sceneStartTime = Time.unscaledTime;
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (loadingGame || Time.unscaledTime - sceneStartTime < inputDelay)
        {
            return;
        }

        if (ContinuePressed())
        {
            loadingGame = true;
            SceneManager.LoadScene(gameSceneName);
        }
    }

    private static bool ContinuePressed()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            return true;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(KeyCode.Space);
#else
        return false;
#endif
    }
}
