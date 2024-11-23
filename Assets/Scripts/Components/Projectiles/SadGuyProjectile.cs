using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SadGuyProjectile : AProjectile
{
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] float upwardForce = 2.5f;
    Vector3 characterVelocity;
    [HideInInspector] public HealthTankManager healthTank;
    [SerializeField] float maxHealing;

    protected override void Awake()
    {
        base.Awake();
        characterVelocity = transform.parent.parent.GetComponent<CharacterController>().velocity;
        healthTank = transform.parent.parent.GetComponent<HealthTankManager>();
        rb.velocity = this.transform.forward * speed + Vector3.up * upwardForce;
        this.transform.parent = null; //Se desvincula del padre para que no le afecte su movimiento

        if (healthTank.Energy < maxHealing)
        {
            healing = healthTank.Energy;
        }
        else
        {
            healing = maxHealing;
        }
    }

    public override void CheckDestroy(Collider other) //Cada proyectil tiene sus condiciones de destrucción
    {
        PlayerBehaviour receiver = other.GetComponent<PlayerBehaviour>();
        if (receiver != GetAttacker())
        {
            IAttack projectile = GameObject.Instantiate(explosionPrefab, transform).GetComponent<IAttack>();
            projectile.SetTag(this.tag); //Le pone tag para que gestione colisiones, daño y curas
            projectile.SetAttacker(GetAttacker()); //Se configura para que sepa quién lanzó el ataque

            if (receiver != null)
            {
                if (!tag.Equals(other.tag))
                {
                    healthTank.UpdateHealthTank("damage", GetDamage());
                }
                else
                {
                    healthTank.UpdateHealthTank("heal", GetHealing());
                }
            }

            Destroy(this.gameObject);
        }
    }
}
