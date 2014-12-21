using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingsMenu : MonoBehaviour 
{
    public GameObject root;
    public Scrollbar acceleration, quality, bightness;
    public int qualityLevel;

    void Start() { OnEnabled(); }

    void OnEnabled() 
    {
        root.SetActive(false);

        // Init Acceleration
        acceleration.value = PlayerPrefs.GetFloat("Input.Acceleration", 5) / 10;

        // Init Quality
        var defaultQ = QualitySettings.GetQualityLevel();
        qualityLevel = PlayerPrefs.GetInt("Video.Quality", defaultQ);
        quality.value = qualityLevel / (QualitySettings.names.Length - 1);
        ApplyQuality();

        // Init Bightness
        var ambient = RenderSettings.ambientLight;
        var defaultB = (ambient.r + ambient.g + ambient.b) / 3;
        bightness.value = PlayerPrefs.GetFloat("Video.Bightness", defaultB);
        SetBrightness();
    }

    public void OnOff()
    {
        root.SetActive(!root.activeSelf);
    }

    public void SetAcceleration() 
    {
        PlayerPrefs.SetFloat("Input.Acceleration", acceleration.value * 10);
    }

    public void SetQuality() 
    {
        var max = QualitySettings.names.Length - 1;
        qualityLevel = Mathf.RoundToInt(quality.value * max);
    }

    public void ApplyQuality() 
    {
        PlayerPrefs.SetInt("Video.Quality", qualityLevel);
        QualitySettings.SetQualityLevel(qualityLevel, true);
    }

    public void SetBrightness() 
    {
        var level = bightness.value;
        PlayerPrefs.SetFloat("Video.Bightness", level);
        RenderSettings.ambientLight = new Color(level, level, level);
    }
}
