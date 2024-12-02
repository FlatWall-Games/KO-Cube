using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System;
using UnityEngine.InputSystem;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Collections.LowLevel.Unsafe;

public class HealthManager : NetworkBehaviour
{
    [SerializeField] private float maxHealth; //Máximo de vida que tiene el jugador
    [SerializeField] private Image healthBar; //Imagen de la barra de vida
    DeathMatchManager deathMatch;
    Animator anim;

    private float currentHealth; //Vida actual
    public EventHandler<string> OnDead;
    public EventHandler<float> OnDamaged;
    public MatchStatsManager matchStatsManager;

    void Awake()
    {
        currentHealth = maxHealth;
        matchStatsManager = GetComponent<MatchStatsManager>();
        deathMatch = GameObject.FindObjectOfType<DeathMatchManager>();
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; //Sólo calcula daños el servidor
        IAttack attack = other.GetComponent<IAttack>();
        if(attack != null && !IsWallBetween(attack)) ManageDamageAndHeal(attack);
    }

    public void OnRaycastHit(IAttack attack)
    {
        if (!IsServer) return; //Sólo calcula daños el servidor
        ManageDamageAndHeal(attack);
    }

    public void ManageDamageAndHeal(IAttack attack)
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
                anim.SetTrigger("Dead");
                GetComponent<CharacterController>().enabled = false; //No queremos recibir más golpes estando muertos
                if (deathMatch != null) deathMatch.PointScored(attack.GetAttacker().tag);
                killed = true;
                attack.GetAttacker().AddKillsClientRpc();
            }
        }
        UpdateHealthClientRpc(currentHealth, killed); //Se actualiza la vida en todos los clientes
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
            if (IsOwner) GetComponent<PlayerInput>().enabled = false;
            matchStatsManager.AddDeath();
            OnDead?.Invoke(this.gameObject, this.tag);
        }
        healthBar.gameObject.SetActive(!killed); //Si muere se le desactiva la barra de vida y al reaparecer se vuelve a activar
    }

    public void RequestRespawn()
    {        
        if (!IsServer) return;
        GetComponent<AttackManager>().OnShootEnded();
        GetComponent<PlayerBehaviour>().InitializePosition();
        currentHealth = maxHealth;
        UpdateHealthClientRpc(currentHealth, false);
        OnRespawnClientRpc();
    }

    [ClientRpc]
    private void OnRespawnClientRpc()
    {
        if (IsOwner)
        {
            GetComponent<PlayerInput>().enabled = true;
            transform.GetComponentInChildren<BasicAmmoManager>().RestoreBarOnSpawn();
        }
    }

    private bool IsWallBetween(IAttack attack) //Comprueba si un ataque a melé se realiza a través de una pared o no
    {
        if (!attack.IsMele()) return false; //El problema sólo lo tienen los ataques a melé
        //Se calcula dirección y distancia con el eje Y igualado:
        Vector3 direction = attack.GetAttacker().transform.position - this.transform.position;
        float distance = Vector3.Distance(attack.GetAttacker().transform.position, this.transform.position);
        if(Physics.Raycast(this.transform.position, direction, distance, 1 << 0))
        {
            return true;
        }
        return false;
    }
}
