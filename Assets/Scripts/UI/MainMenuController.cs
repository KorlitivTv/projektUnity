using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject storyPanel;
    [SerializeField] private GameObject optionsPanel;

    private void Start()
    {
        ShowMainMenu();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void ShowMainMenu()
    {
        SetPanel(mainMenuPanel, true);
        SetPanel(storyPanel, false);
        SetPanel(optionsPanel, false);
    }

    public void ShowStory()
    {
        SetPanel(mainMenuPanel, false);
        SetPanel(storyPanel, true);
        SetPanel(optionsPanel, false);
    }

    public void ShowOptions()
    {
        SetPanel(mainMenuPanel, false);
        SetPanel(storyPanel, false);
        SetPanel(optionsPanel, true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void SetPanel(GameObject panel, bool active)
    {
        if (panel != null)
        {
            panel.SetActive(active);
        }
    }
}
