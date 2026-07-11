using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string storySceneName = "Story";

    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject storyPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;

    private void Start()
    {
        ShowMainMenu();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(storySceneName);
    }

    public void ShowMainMenu()
    {
        SetOnlyPanel(mainMenuPanel);
    }

    public void ShowStory()
    {
        SetOnlyPanel(storyPanel);
    }

    public void ShowOptions()
    {
        SetOnlyPanel(optionsPanel);
    }

    public void ShowCredits()
    {
        SetOnlyPanel(creditsPanel);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void SetOnlyPanel(GameObject panelToShow)
    {
        SetPanel(mainMenuPanel, panelToShow == mainMenuPanel);
        SetPanel(storyPanel, panelToShow == storyPanel);
        SetPanel(optionsPanel, panelToShow == optionsPanel);
        SetPanel(creditsPanel, panelToShow == creditsPanel);
    }

    private static void SetPanel(GameObject panel, bool active)
    {
        if (panel != null)
        {
            panel.SetActive(active);
        }
    }
}
