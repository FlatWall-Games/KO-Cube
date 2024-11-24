using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudyProjectile : AProjectile
{
    [SerializeField] private float effectiveTime = 0.3f;

    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(HideAttack());
    }

    IEnumerator HideAttack()
    {
        yield return new WaitForSeconds(effectiveTime);
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;
    }
}
