using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
//using UnityEditor.Experimental.GraphView;

public class HealthManager : NetworkBehaviour
{
    [SerializeField] private float maxHealth; //M�ximo de vida que tiene el jugador
    [SerializeField] private Image healthBar; //Imagen de la barra de vida
    private float currentHealth; //Vida actual

    void Awake()
    {
        currentHealth = maxHealth;
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
        string type;
        float amount;

        HealthTankManager sadGuy = attack.GetAttacker().gameObject.GetComponent<HealthTankManager>();

        if (attack != null)
        {
            if (attack.GetAttacker() == GetComponent<PlayerBehaviour>()) return; //Los propios b�sicos no afectan a uno mismo
            if (attack.GetTag().Equals(this.tag)) //Si es procedente del equipo el b�sico puede curar
            {
                float finalHealing = attack.GetHealing();
                if(sadGuy != null)
                {
                    if (attack.GetHealing() > sadGuy.Energy)
                    {
                        finalHealing = sadGuy.Energy;
                    }
                }
                currentHealth += finalHealing;
                amount = finalHealing;
                if (currentHealth > maxHealth) 
                { 
                    amount = attack.GetHealing() - (currentHealth - maxHealth);
                    currentHealth = maxHealth; 
                }

                type = "heal";
            }
            else //Si no, da�an
            {
                currentHealth -= attack.GetDamage();
                amount = attack.GetDamage();
                if (currentHealth <= 0) 
                {
                    amount = attack.GetDamage() + currentHealth;
                    currentHealth = 0; 
                    Debug.Log("MUERTO"); 
                }

                type = "damage";
            }
            UpdateHealthClientRpc(currentHealth); //Se actualiza la vida en todos los clientes

            if (sadGuy == null) return;

            sadGuy.UpdateHealthTank(type, amount);
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
