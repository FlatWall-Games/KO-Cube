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
    DeathMatchManager deathMatch;

    private float currentHealth; //Vida actual
    public EventHandler<string> OnDead;
    public EventHandler<float> OnDamaged;
    public MatchStatsManager matchStatsManager;

    void Awake()
    {
        currentHealth = maxHealth;
        matchStatsManager = GetComponent<MatchStatsManager>();
        deathMatch = GameObject.FindObjectOfType<DeathMatchManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; //Sólo calcula daños el servidor
        IAttack attack = other.GetComponent<IAttack>();
        ManageDamageAndHeal(attack);
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

            bool killed = false;
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
                OnDamaged?.Invoke(this.gameObject, attack.GetDamage());
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

    public void ForceHeal(float amount) //Permite la cura sin proyectil de por medio
    {
        if (currentHealth <= 0) return; //No se puede curar si ha muerto
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateHealthClientRpc(currentHealth, false);
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
