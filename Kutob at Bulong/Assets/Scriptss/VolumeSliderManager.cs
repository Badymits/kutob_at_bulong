using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VolumeSliderManager : MonoBehaviour
{
    public Slider volumeSlider;
    public TextMeshProUGUI volumeText;
    public AudioSource[] audioSources;

    void Start()
    {
        volumeSlider.onValueChanged.AddListener(HandleVolumeChange);
        volumeSlider.value = 1f;  // Start at full volume
    }

    void HandleVolumeChange(float value)
    {
        foreach (AudioSource source in audioSources)
        {
            source.volume = value;
        }
        volumeText.text = Mathf.RoundToInt(value * 100).ToString();  // Show as percentage
    }
}