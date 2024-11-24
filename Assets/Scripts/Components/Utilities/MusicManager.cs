using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] musicClips;
    [SerializeField] private AudioClip[] ambientClips;
    [SerializeField] private AudioMixerGroup musicMixer;
    [SerializeField] private float currentVolume = 0;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambientSource;
    private Animator anim;
    private bool animationActive;
    private int currentSong = 0;
    private bool loop = true;
    
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (animationActive)
        {
            musicMixer.audioMixer.SetFloat("Volume", currentVolume);
        }
    }

    public void PlaySong(int songIndex, float animationSpeed, bool loop)
    {
        this.loop = loop;
        musicSource.loop = true;
        anim.speed = animationSpeed;
        currentSong = songIndex;
        anim.Rebind();
        Debug.Log("Reproducida canción número: " + songIndex);
    }

    public void PlayAmbient(int ambientIndex, bool loop)
    {
        ambientSource.loop = loop;
        ambientSource.clip = ambientClips[ambientIndex];
        ambientSource.Play();
    }

    public void ChangeSong() //Llamada desde la animación de fade-out fade-in
    {
        musicSource.clip = musicClips[currentSong];
        musicSource.Play();
        musicSource.loop = loop;
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
