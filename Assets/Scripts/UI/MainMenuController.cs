using TMPro;
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
    [SerializeField] private GameObject controlsPanel;

    [Header("Main Menu Stats")]
    [SerializeField] private TextMeshProUGUI bestScoreText;

    private const string BestScoreKey = "BestScore";

    private void Start()
    {
        ShowMainMenu();
        RefreshBestScore();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(storySceneName);
    }

    public void ShowMainMenu()
    {
        SetOnlyPanel(mainMenuPanel);
        RefreshBestScore();
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

    public void ShowControls()
    {
        SetOnlyPanel(controlsPanel);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void RefreshBestScore()
    {
        if (bestScoreText != null)
        {
            int bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
            bestScoreText.text = "Best Score: " + bestScore;
        }
    }

    private void SetOnlyPanel(GameObject panelToShow)
    {
        SetPanel(mainMenuPanel, panelToShow == mainMenuPanel);
        SetPanel(storyPanel, panelToShow == storyPanel);
        SetPanel(optionsPanel, panelToShow == optionsPanel);
        SetPanel(creditsPanel, panelToShow == creditsPanel);
        SetPanel(controlsPanel, panelToShow == controlsPanel);
    }

    private static void SetPanel(GameObject panel, bool active)
    {
        if (panel != null)
        {
            panel.SetActive(active);
        }
    }
}
