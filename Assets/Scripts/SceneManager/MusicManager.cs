using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();

            LoadVolumeSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
        SaveVolume(volume); 
    }

    private void LoadVolumeSettings()
    {
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f); 
        SetVolume(savedVolume); 
    }

    private void SaveVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }
}
