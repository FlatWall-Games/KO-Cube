using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HealthManager : NetworkBehaviour
{
    [SerializeField] private float maxHealth; //Máximo de vida que tiene el jugador
    private float currentHealth; //Vida actual

    void Awake()
    {
        currentHealth = maxHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        IAttack attack = other.GetComponent<IAttack>();
        if (attack != null)
        {
            if (attack.GetTag().Equals(this.tag))
            {
                currentHealth += attack.GetHealing();
                if (currentHealth > maxHealth) currentHealth = maxHealth;
            }
            else
            {
                currentHealth -= attack.GetDamage();
                if (currentHealth <= 0) Debug.Log("MUERTO");
            }
            DamagedClientRpc(currentHealth);
        }
    }

    [ClientRpc]
    private void DamagedClientRpc(float newHealth)
    {
        currentHealth = newHealth;
        Debug.Log(currentHealth);
    }
}
