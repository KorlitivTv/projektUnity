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

    private void Awake()
    {
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, defaultVolume);
        bool savedFullscreen = PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1;

        ApplyVolume(savedVolume, save: false);
        ApplyFullscreen(savedFullscreen, save: false);

        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.SetValueWithoutNotify(savedVolume);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.SetIsOnWithoutNotify(savedFullscreen);
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

    private static void ApplyFullscreen(bool enabled, bool save)
    {
        Screen.fullScreen = enabled;

        if (save)
        {
            PlayerPrefs.SetInt(FullscreenKey, enabled ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
