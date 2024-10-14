using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachinganProjectile : AProjectile
{
    protected override void Awake()
    {
        base.Awake();
        Transform parent = transform.parent;
        rb.velocity = this.transform.forward * speed;
        this.transform.parent = null; //Se desvincula del padre para que no le afecte su movimiento
        parent.localPosition = new Vector3(-parent.localPosition.x, parent.localPosition.y, parent.localPosition.z);
    }

    public override void CheckDestroy(string otherTag) //Cada proyectil tiene sus condiciones de destrucción
    {
        //En este caso, el proyectil se destruye al chocar con un jugador del otro equipo o con un objeto del mapa
        if (otherTag.Equals("Team1"))
        {
            if (this.tag.Equals("Team2")) Destroy(this.gameObject);
        }
        else if (otherTag.Equals("Team2"))
        {
            if (this.tag.Equals("Team1")) Destroy(this.gameObject);
        }
        else Destroy(this.gameObject);
    }
}
