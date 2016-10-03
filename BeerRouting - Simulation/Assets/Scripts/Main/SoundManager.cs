using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public bool debug = false;
    public AudioSource audioBarNoise;
    public AudioSource audioRunComplete;
    public List<AudioSource> audioBottleClick;
    public AudioSource audioCollectible1;
    public AudioSource audioCollectible2;
    public AudioSource audioCollectible3;
    public AudioSource audioPlayerJump;
    public AudioSource audioPlayerDead;
    public AudioSource audioPlayerWater;
    public AudioSource audioMeow;

    public enum SoundType
    {
        BarNoise,
        RunComplete,
        BottleClick,
        Collectible1,
        Collectible2,
        Collectible3,
        PlayerJump,
        PlayerDead,
        PlayerWater,
        Meow
    }

    void Start()
    {
        // Play bar sound on start in each level.
        PlaySound(SoundType.BarNoise);
    }

    public void PlaySound(SoundType soundType)
    {
        if (debug)
            Debug.Log("SoundManager - PlaySound: " + soundType);
        switch (soundType)
        {
            case SoundType.BarNoise:
                audioBarNoise.Play();
                break;
            case SoundType.RunComplete:
                audioRunComplete.Play();
                break;
            case SoundType.BottleClick:
                // Play random beer bottle sound.
                int rand = Random.Range(0, audioBottleClick.Count);
                audioBottleClick[rand].Play();
                if (debug)
                    Debug.Log("SoundManager - BottleClick: " + rand);
                break;
            case SoundType.Collectible1:
                audioCollectible1.Play();
                break;
            case SoundType.Collectible2:
                audioCollectible2.Play();
                break;
            case SoundType.Collectible3:
                audioCollectible3.Play();
                break;
            case SoundType.PlayerDead:
                audioPlayerDead.Play();
                break;
            case SoundType.PlayerJump:
                audioPlayerJump.Play();
                break;
            case SoundType.PlayerWater:
                audioPlayerWater.Play();
                break;
            case SoundType.Meow:
                audioMeow.Play();
                break;
        }
    }
}
