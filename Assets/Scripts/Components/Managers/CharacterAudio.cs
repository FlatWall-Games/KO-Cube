using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAudio : MonoBehaviour
{
    [SerializeField] private AudioSource basicSource;
    [SerializeField] private AudioSource ultSource;
    [SerializeField] private AudioSource damagedSource;
    [SerializeField] private AudioSource killedSource;
    private List<float> initialVolumes;
    private float maxDistance = 45; //Distancia máxima a la que se escucha el sonido

    void Awake()
    {
        transform.parent.GetComponent<HealthManager>().OnDead += PlayKilledAudio;
        transform.parent.GetComponent<HealthManager>().OnDamaged += PlayDamagedAudio;
        transform.parent.GetComponent<AttackManager>().OnAttack += PlayBasicAudio;
        initialVolumes = new List<float>() { basicSource.volume, ultSource.volume, damagedSource.volume, killedSource.volume };
    }

    private void PlayKilledAudio(object s, string tag)
    {
        //Cuando muere se paran el resto de audios:
        basicSource.Stop();
        ultSource.Stop();
        damagedSource.Stop();
        AdjustVolume(killedSource, 3);
        killedSource.Play();
    }

    private void PlayDamagedAudio(object s, float damage)
    {
        //damagedSource.Play();
    }

    private void PlayBasicAudio(object s, string attackType)
    {
        if (attackType.Equals("BASIC"))
        {
            AdjustVolume(basicSource, 0);
            basicSource.Play();
        }
        else
        {
            AdjustVolume(ultSource, 1);
            ultSource.Play();
        }
    }

    private void AdjustVolume(AudioSource source, int listIndex)
    {
        Vector3 audioListenerPosition = GameObject.FindObjectOfType<AudioListener>().transform.position;
        float distanceFactor = Mathf.Clamp(1 - Vector3.Distance(transform.position, audioListenerPosition)/maxDistance, 0, 1);
        source.volume = initialVolumes[listIndex] * distanceFactor;
    }
}
