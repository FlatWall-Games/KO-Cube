using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SadGuyProjectileExplosion : AProjectile
{

    protected override void Awake()
    {
        base.Awake();
        this.transform.parent = null; //Se desvincula del padre para que no le afecte su movimiento
    }

    public override void CheckDestroy(string otherTag) //Cada proyectil tiene sus condiciones de destrucción
    {
    }
}
