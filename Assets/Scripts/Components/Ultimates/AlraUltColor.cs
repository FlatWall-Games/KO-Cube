using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlraUltColor : MonoBehaviour
{
    [SerializeField] private GameObject greenParticles;
    [SerializeField] private GameObject redParticles;
    [SerializeField] private Renderer rend;
    public void UpdateColor(string tag) //Llamado al hacer el SetTag en MachinganUlt
    {
        Debug.Log($"{tag}, {PlayerBehaviour.ownerTag}");
        if (!PlayerBehaviour.ownerTag.Equals(tag))
        {
            rend.material = new Material(rend.material);
            rend.material.SetColor("_color", Color.red);
            greenParticles.SetActive(false);
            redParticles.SetActive(true);
        }
    }
}
