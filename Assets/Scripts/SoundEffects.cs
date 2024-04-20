using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip startGameClip;   // Assign clip for 'Player started game'
    public AudioClip countdownClip;   // Assign clip for '3, 2, 1, go!'
    public AudioClip gameOverClip;    // Assign clip for 'Game over'
    public AudioClip collectBoostClip; // Assign clip for 'Collect boost'
    public AudioClip collectCoinClip;  // Assign clip for 'Collect coin'
    public AudioClip winnerClip;       // Assign clip for 'Winner!'
    public AudioClip jumpClip;         // Assign clip for 'Jump!'

    // Method to play the start game sound
    public void PlayStartGame()
    {
        PlaySound(startGameClip);
    }

    // Method to play the countdown sound
    public void PlayCountdown()
    {
        PlaySound(countdownClip);
    }

    // Method to play the game over sound
    public void PlayGameOver()
    {
        PlaySound(gameOverClip);
    }

    // Method to play the collect boost sound
    public void PlayCollectBoost()
    {
        PlaySound(collectBoostClip);
    }

    // Method to play the collect coin sound
    public void PlayCollectCoin()
    {
        PlaySound(collectCoinClip);
    }

    // Method to play the winner sound
    public void PlayWinner()
    {
        PlaySound(winnerClip);
    }

    // Method to play the jump sound
    public void PlayJump()
    {
        PlaySound(jumpClip);
    }

    // Method to play a sound without interrupting others
    private void PlaySound(AudioClip clip)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();
        Destroy(audioSource, clip.length); // Destroy the AudioSource after the clip is done playing
    }
}
