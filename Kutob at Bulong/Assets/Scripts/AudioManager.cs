using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("---------- Audio Source ----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("---------- Audio Clip ----------")]
    public AudioClip background;

    private static AudioManager instance;

    private void Awake()
    {
        // Check if there is already an instance of AudioManager
        if (instance == null)
        {
            // Set the current instance if none exists
            instance = this;

            // Prevent this object from being destroyed when loading a new scene
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy this new instance if one already exists
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}