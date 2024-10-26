using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System;

public class HealthManager : NetworkBehaviour
{
    [SerializeField] private float maxHealth; //Máximo de vida que tiene el jugador
    [SerializeField] private Image healthBar; //Imagen de la barra de vida

    private float currentHealth; //Vida actual
    public event Action OnDead;
    public MatchStatsManager matchStatsManager;

    void Awake()
    {
        currentHealth = maxHealth;
        matchStatsManager = GetComponent<MatchStatsManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; //Sólo calcula daños el servidor
        IAttack attack = other.GetComponent<IAttack>();
        ManageDamageAndHeal(attack);
        if (currentHealth <= 0)
        {
            other.gameObject.GetComponent<AProjectile>().GetAttacker().GetComponent<MatchStatsManager>().AddKill();
        }
    }

    public void OnRaycastHit(IAttack attack)
    {
        if (!IsServer) return; //Sólo calcula daños el servidor
        ManageDamageAndHeal(attack);
    }

    public void ManageDamageAndHeal(IAttack attack)
    {
        if (attack != null)
        {
            if (attack.GetAttacker() == GetComponent<PlayerBehaviour>()) return; //Los propios básicos no afectan a uno mismo
            if (attack.GetTag().Equals(this.tag)) //Si es procedente del equipo el básico puede curar
            {
                currentHealth += attack.GetHealing();

                if (currentHealth > maxHealth) 
                { 
                    currentHealth = maxHealth; 
                }
            }
            else //Si no, dañan
            {
                currentHealth -= attack.GetDamage();

                if (currentHealth <= 0) 
                {
                    OnDead?.Invoke();
                    GetComponent<PlayerBehaviour>().InitializePosition();
                    GameObject.FindObjectOfType<DeathMatchManager>().PlayerKilled(this.tag);
                    matchStatsManager.AddDeath();
                    currentHealth = maxHealth;
                }
            }
            UpdateHealthClientRpc(currentHealth); //Se actualiza la vida en todos los clientes
        }
    }

    [ClientRpc]
    private void UpdateHealthClientRpc(float newHealth)
    {
        currentHealth = newHealth;
        float value = currentHealth / maxHealth;
        healthBar.color = new Color(1 - value, value, 0, 1);
        healthBar.fillAmount = value;
    }
}
