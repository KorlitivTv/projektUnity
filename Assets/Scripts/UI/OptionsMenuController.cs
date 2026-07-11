using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour
{
    private const string VolumeKey = "MasterVolume";
    private const string FullscreenKey = "Fullscreen";

    [Header("UI")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Defaults")]
    [SerializeField, Range(0f, 1f)] private float defaultVolume = 0.7f;
    [SerializeField] private Vector2Int windowedResolution = new Vector2Int(1280, 720);

    private void Awake()
    {
        float savedVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(VolumeKey, defaultVolume));
        bool savedFullscreen = PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1;

        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.wholeNumbers = false;
            volumeSlider.SetValueWithoutNotify(savedVolume);
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.SetIsOnWithoutNotify(savedFullscreen);
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }

        ApplyVolume(savedVolume, save: false);
        ApplyFullscreen(savedFullscreen, save: false);
    }

    private void OnDestroy()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(SetVolume);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.RemoveListener(SetFullscreen);
        }
    }

    public void SetVolume(float value)
    {
        ApplyVolume(value, save: true);
    }

    public void SetFullscreen(bool enabled)
    {
        ApplyFullscreen(enabled, save: true);
    }

    private static void ApplyVolume(float value, bool save)
    {
        float clampedValue = Mathf.Clamp01(value);
        AudioListener.volume = clampedValue;

        if (save)
        {
            PlayerPrefs.SetFloat(VolumeKey, clampedValue);
            PlayerPrefs.Save();
        }
    }

    private void ApplyFullscreen(bool enabled, bool save)
    {
        if (enabled)
        {
            Resolution resolution = Screen.currentResolution;
            Screen.SetResolution(
                resolution.width,
                resolution.height,
                FullScreenMode.FullScreenWindow
            );
        }
        else
        {
            int width = Mathf.Max(640, windowedResolution.x);
            int height = Mathf.Max(360, windowedResolution.y);
            Screen.SetResolution(width, height, FullScreenMode.Windowed);
        }

        if (save)
        {
            PlayerPrefs.SetInt(FullscreenKey, enabled ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
