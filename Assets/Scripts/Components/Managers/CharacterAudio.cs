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

    void Awake()
    {
        transform.parent.GetComponent<HealthManager>().OnDead += PlayKilledAudio;
        transform.parent.GetComponent<HealthManager>().OnDamaged += PlayDamagedAudio;
        transform.parent.GetComponent<AttackManager>().OnAttack += PlayBasicAudio;
    }

    private void PlayKilledAudio(object s, string tag)
    {
        //Cuando muere se paran el resto de audios:
        basicSource.Stop();
        ultSource.Stop();
        damagedSource.Stop();
        killedSource.Play();
    }

    private void PlayDamagedAudio(object s, float damage)
    {
        //damagedSource.Play();
    }

    private void PlayBasicAudio(object s, string attackType)
    {
        if (attackType.Equals("BASIC")) basicSource.Play();
        else ultSource.Play();
    }
}
