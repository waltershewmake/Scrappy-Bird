using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeMusic : MonoBehaviour
{
    public AudioSource source;

    public AudioClip normalClip;  // Assign theme.mp3 in the Unity editor
    public AudioClip slowClip;    // Assign theme_slow.mp3 in the Unity editor
    public AudioClip fastClip;    // Assign theme_fast.mp3 in the Unity editor

    // Start is called before the first frame update
    void Start()
    {
        //PlayNormalTheme(); // Default to playing the normal theme at the start
    }

    // Update is called once per frame
    void Update()
    {
        // Update loop left intentionally empty
        PlayNormalTheme();
    }

    // Method to play the normal theme
    public void PlayNormalTheme()
    {
        if (source.isPlaying) source.Stop();
        source.clip = normalClip;
        source.Play();
    }

    // Method to play the slow theme
    public void PlaySlowTheme()
    {
        if (source.isPlaying) source.Stop();
        source.clip = slowClip;
        source.Play();
    }

    // Method to play the fast theme
    public void PlayFastTheme()
    {
        if (source.isPlaying) source.Stop();
        source.clip = fastClip;
        source.Play();
    }
}
