using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleoPassive : MonoBehaviour
{
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private Transform basicTransform;
    private float counter = 0;

    public void AddHit()
    {
        if(++counter == 3)
        {
            counter = 0;
            GameObject attack = GameObject.Instantiate(attackPrefab, basicTransform);
            attack.tag = this.tag;
        }
    }

    public void ResetCounter()
    {
        counter = 0;
    }
}
