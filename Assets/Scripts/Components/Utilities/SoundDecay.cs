using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundDecay : MonoBehaviour
{
    private float maxDistance = 45;

    private void Awake()
    {
        Vector3 listenerPosition = GameObject.FindObjectOfType<AudioListener>().transform.position;
        float distanceFactor = 1 - Mathf.Clamp(Vector3.Distance(this.transform.position, listenerPosition)/maxDistance, 0, 1);
        GetComponent<AudioSource>().volume *= distanceFactor;
        GetComponent<AudioSource>().Play();
    }
}
