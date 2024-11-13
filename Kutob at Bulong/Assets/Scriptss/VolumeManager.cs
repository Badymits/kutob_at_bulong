using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VolumeManager : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;  // Will show in Inspector
    [SerializeField] private TextMeshProUGUI volumeText;  // Will show in Inspector
    [SerializeField] private AudioSource[] audioSources;  // Will show in Inspector

    void Start()
    {
        // Set initial volume text
        UpdateVolumeText(volumeSlider.value);

        // Add listener for when slider value changes
        volumeSlider.onValueChanged.AddListener(UpdateVolumeText);
    }

    void UpdateVolumeText(float value)
    {
        // Convert 0-1 to 0-100 and round to whole number
        int volumeValue = Mathf.RoundToInt(value * 100);
        volumeText.text = volumeValue.ToString();

        // Update audio volume
        if (audioSources != null)
        {
            foreach (AudioSource source in audioSources)
            {
                if (source != null)
                    source.volume = value;
            }
        }
    }
}