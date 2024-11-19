using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private AudioMixerGroup musicMixer;
    [SerializeField] private float currentVolume = 0;
    private AudioSource source;
    private Animator anim;
    private bool animationActive;
    private int currentSong = 0;
    
    void Awake()
    {
        source = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (animationActive)
        {
            musicMixer.audioMixer.SetFloat("Volume", currentVolume);
        }
    }

    public void PlaySong(int songIndex, float animationSpeed)
    {
        anim.speed = animationSpeed;
        currentSong = songIndex;
        anim.Rebind();
    }

    public void ChangeSong()
    {
        source.clip = clips[currentSong];
        source.Play();
    }

    public void OnAnimationStarted()
    {
        animationActive = true;
    }

    public void OnAnimationEnded()
    {
        animationActive = false;
    }
}
