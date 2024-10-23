using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SadGuyProjectileExplosion : AProjectile
{

    protected override void Awake()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        base.Awake();
        this.transform.parent = null; //Se desvincula del padre para que no le afecte su movimiento
    }

    public override void CheckDestroy(Collider other) //Cada proyectil tiene sus condiciones de destrucción
    {
        if (!NetworkManager.Singleton.IsServer) return;
    }
}
