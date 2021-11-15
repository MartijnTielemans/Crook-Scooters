using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSoundManager : MonoBehaviour
{
    [SerializeField]
    AudioSource[] soundEffects;

    [SerializeField]
    AudioSource[] laughSounds;

    public void PlaySound(int sound)
    {
        soundEffects[sound].Play();
    }

    public void PlayLaugh(int laugh)
    {
        laughSounds[laugh].Play();
    }
}
