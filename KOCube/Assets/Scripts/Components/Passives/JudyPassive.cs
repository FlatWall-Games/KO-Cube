using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class JudyPassive : NetworkBehaviour
{
    private bool canRestore = true;

    void Awake()
    {
        GetComponent<HealthManager>().OnDamaged += RestoreHealth;
    }

    private void RestoreHealth(object s, float healthToRecover)
    {
        if (!IsServer) return;
        canRestore = !canRestore;
        if (canRestore) GetComponent<HealthManager>().ForceHeal(healthToRecover);
    }
}
