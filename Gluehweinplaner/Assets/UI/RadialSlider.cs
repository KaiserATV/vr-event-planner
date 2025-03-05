// Create a new script RadialSlider.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RadialSlider : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI valueText;
    public VolumeManager volumeManager;
    
    void Start()
    {
        slider.onValueChanged.AddListener(UpdateVolume);
        slider.value = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
    }

    void UpdateVolume(float value)
    {
        volumeManager.SetMasterVolume(value);
        valueText.text = $"{Mathf.RoundToInt(value * 100)}%";
    }
}