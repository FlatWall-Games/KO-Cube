using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] effects;
    [SerializeField] private AudioSource sfxSource;

    public void PlaySFX(int sfxIndex, float volume)
    {
        sfxSource.volume = volume;
        sfxSource.PlayOneShot(effects[sfxIndex]);
    }
}
