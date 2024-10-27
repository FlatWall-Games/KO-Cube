using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SadGuyProjectileExplosion : AProjectile
{
    HealthTankManager healthTank;
    float maxHealing = 15f;

    protected override void Awake()
    {
        base.Awake();
        healthTank = transform.parent.GetComponent<SadGuyProjectile>().healthTank;
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
    }
}
