using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ThemeMusic : MonoBehaviour
{
    public AudioSource source;

    public AudioClip normalClip;  // Assign theme.mp3 in the Unity editor
    public AudioClip slowClip;    // Assign theme_slow.mp3 in the Unity editor
    public AudioClip fastClip;    // Assign theme_fast.mp3 in the Unity editor
    public AudioClip starPowerClip;  // Assign star_power.mp3 in the Unity editor

    private AudioClip lastClip;   // To keep track of the last played clip

    // Start is called before the first frame update
    void Start()
    {
        PlayNormalTheme(); // Default to playing the normal theme at the start
    }

    // Update is called once per frame
    void Update()
    {
        // Update loop left intentionally empty
    }
    
    public void StopPlaying()
    {
        source.Stop();
    }

    // Method to play the normal theme
    public void PlayNormalTheme()
    {
        PlayTheme(normalClip);
    }

    // Method to play the slow theme
    public void PlaySlowTheme()
    {
        PlayTheme(slowClip);
    }

    // Method to play the fast theme
    public void PlayFastTheme()
    {
        PlayTheme(fastClip);
    }

    // Generic method to handle playing any theme
    private void PlayTheme(AudioClip clip)
    {
        // if source is already the same clip, don't play it again
        if (lastClip == clip) return;
        
        if (source.isPlaying) source.Stop();
        source.clip = clip;
        source.Play();
        lastClip = clip;  // Keep track of the last played clip
    }

    // Method to play the star power theme for a specified duration
    public void PlayStarPowerTheme(float duration)
    {
        StartCoroutine(PlayStarPower(duration));
    }

    // Coroutine to manage star power theme playback
    private IEnumerator PlayStarPower(float duration)
    {
        // Pause current theme and remember it
        AudioClip currentClip = source.clip;
        source.Pause();

        // Play the star power theme
        source.clip = starPowerClip;
        source.Play();

        // Wait for the duration of the star power theme
        yield return new WaitForSeconds(duration);

        // Stop the star power theme and resume the original theme
        source.Stop();
        source.clip = currentClip;
        source.Play();
    }
}
