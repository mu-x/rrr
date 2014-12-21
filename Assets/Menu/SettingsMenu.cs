using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingsMenu : MonoBehaviour 
{
    public GameObject root;
    public Scrollbar acceleration, quality, bightness;

    void OnEnabled()
    {
        root.SetActive(false);
    }

    public void OnOff()
    {
        root.SetActive(!root.activeSelf);
    }

    void Start()
    {
        OnEnabled();

        // Init Acceleration
        acceleration.value = PlayerPrefs.GetFloat("Input.Acceleration", 5) / 10;
       
        // Init Quality
        quality.numberOfSteps = QualitySettings.names.Length;
        quality.value = QualitySettings.GetQualityLevel() / 
            quality.numberOfSteps;

        // Init Bightness
        var c = RenderSettings.ambientLight;
        bightness.value = (c.r + c.g + c.b) / 3;
    }

    public void SetAcceleration() 
    {
        PlayerPrefs.SetFloat("Input.Acceleration", acceleration.value * 10);
    }

    public void SetQuality() 
    {
        var level = Mathf.RoundToInt(quality.value * QualitySettings.names.Length);
        QualitySettings.SetQualityLevel(level, true);
    }

    public void SetBrightness() 
    {
        var level = bightness.value;
        RenderSettings.ambientLight = new Color(level, level, level);
            
            //= Color.Lerp(ambientDarkest, ambientLightest, sliderValue);
    }
}
