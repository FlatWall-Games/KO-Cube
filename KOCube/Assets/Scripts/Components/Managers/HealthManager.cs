using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System;

public class HealthManager : NetworkBehaviour
{
    [SerializeField] private float maxHealth; //M�ximo de vida que tiene el jugador
    [SerializeField] private Image healthBar; //Imagen de la barra de vida
    DeathMatchManager deathMatch;

    private float currentHealth; //Vida actual
    public EventHandler<string> OnDead;
    public MatchStatsManager matchStatsManager;

    void Awake()
    {
        currentHealth = maxHealth;
        matchStatsManager = GetComponent<MatchStatsManager>();
        deathMatch = GameObject.FindObjectOfType<DeathMatchManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; //S�lo calcula da�os el servidor
        IAttack attack = other.GetComponent<IAttack>();
        ManageDamageAndHeal(attack);
    }

    public void OnRaycastHit(IAttack attack)
    {
        if (!IsServer) return; //S�lo calcula da�os el servidor
        ManageDamageAndHeal(attack);
    }

    public void ManageDamageAndHeal(IAttack attack)
    {
        if (attack != null)
        {
            if (attack.GetAttacker() == GetComponent<PlayerBehaviour>()) return; //Los propios b�sicos no afectan a uno mismo

            bool killed = false;
            if (attack.GetTag().Equals(this.tag)) //Si es procedente del equipo el b�sico puede curar
            {
                currentHealth += attack.GetHealing();

                if (currentHealth > maxHealth) 
                { 
                    currentHealth = maxHealth; 
                }
            }
            else //Si no, da�an
            {
                currentHealth -= attack.GetDamage();

                if (currentHealth <= 0) 
                {
                    OnDead?.Invoke(this.gameObject, this.tag);
                    GetComponent<PlayerBehaviour>().InitializePosition();
                    if(deathMatch != null) deathMatch.PointScored(attack.GetAttacker().tag);
                    killed = true;
                    attack.GetAttacker().AddKillsClientRpc();
                    currentHealth = maxHealth;
                }
            }
            UpdateHealthClientRpc(currentHealth, killed); //Se actualiza la vida en todos los clientes
        }
    }

    [ClientRpc]
    private void UpdateHealthClientRpc(float newHealth, bool killed)
    {
        currentHealth = newHealth;
        float value = currentHealth / maxHealth;
        healthBar.color = new Color(1 - value, value, 0, 1);
        healthBar.fillAmount = value;
        if (killed)
        {
            matchStatsManager.AddDeath();
        }
    }
}
