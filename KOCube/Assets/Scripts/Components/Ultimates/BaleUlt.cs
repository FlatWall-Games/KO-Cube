using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaleUlt : AProjectile
{

    protected override void Awake()
    {
        //if (!NetworkManager.Singleton.IsServer) return;

        base.Awake();
        rb.velocity = this.transform.forward * speed;
        this.transform.parent = null; //Se desvincula del padre para que no le afecte su movimiento
    }

    public override void CheckDestroy(Collider other) //Cada proyectil tiene sus condiciones de destrucción
    {
        //if (!NetworkManager.Singleton.IsServer) return;

        string otherTag = other.tag;
        if (otherTag.Equals(this.tag))
            return;
        this.damage += 10; 
    }
}
