using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEditor.Experimental.GraphView;

public class HealthManager : NetworkBehaviour
{
    [SerializeField] private float maxHealth; //Máximo de vida que tiene el jugador
    [SerializeField] private Image healthBar; //Imagen de la barra de vida
    private Camera cam; //Cámara principal
    private float currentHealth; //Vida actual

    void Awake()
    {
        currentHealth = maxHealth;
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    private void Update()
    {
        healthBar.transform.forward = healthBar.transform.position - cam.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; //Sólo calcula daños el servidor
        IAttack attack = other.GetComponent<IAttack>();
        if (attack != null)
        {
            if (attack.GetAttacker() == GetComponent<PlayerMovement>()) return;
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
            UpdateHealthClientRpc(currentHealth);
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
