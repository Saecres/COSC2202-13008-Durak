using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour
{
    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(HandleSliderChange);

        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f); 
        slider.value = savedVolume * 100;
    }

    private void HandleSliderChange(float value)
    {
        float scaledValue = value / 100; 
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetVolume(scaledValue);
        }

        // Save the volume setting, storing it as 0-1
        PlayerPrefs.SetFloat("MusicVolume", scaledValue);
        PlayerPrefs.Save();
    }
}
