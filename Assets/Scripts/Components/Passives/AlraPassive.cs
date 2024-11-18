using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AlraPassive : NetworkBehaviour
{
    [SerializeField] private float healthRecoverAmount;
    [SerializeField] private float interval;
    private HealthManager healthManager;

    void Awake()
    {
        healthManager = GetComponent<HealthManager>();
    }

    private void Start()
    {
        if (IsServer) StartCoroutine(ConstantHeal());
    }

    IEnumerator ConstantHeal()
    {
        while (true)
        {
            healthManager.ForceHeal(healthRecoverAmount);
            yield return new WaitForSeconds(interval);
        }
    }
}
