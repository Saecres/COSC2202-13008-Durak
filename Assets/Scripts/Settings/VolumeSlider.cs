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
    }

    private void HandleSliderChange(float value)
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetVolume(value);
        }

        // Save the volume setting
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }
}
