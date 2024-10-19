using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SadGuyProjectile : AProjectile
{
    [SerializeField] GameObject explosionPrefab;
    Vector3 characterVelocity;

    protected override void Awake()
    {
        base.Awake();
        characterVelocity = transform.parent.parent.GetComponent<CharacterController>().velocity;
        rb.velocity = this.transform.forward * speed;
        rb.AddForce(characterVelocity, ForceMode.Impulse);
        this.transform.parent = null; //Se desvincula del padre para que no le afecte su movimiento
    }

    public override void CheckDestroy(string otherTag) //Cada proyectil tiene sus condiciones de destrucción
    {
        IAttack projectile = GameObject.Instantiate(explosionPrefab, transform.position, transform.rotation).GetComponent<IAttack>();
        projectile.SetTag(this.tag); //Le pone tag para que gestione colisiones, daño y curas
        projectile.SetAttacker(GetAttacker()); //Se configura para que sepa quién lanzó el ataque

        //En este caso, el proyectil se destruye al chocar con un jugador del otro equipo o con un objeto del mapa
        if (otherTag.Equals("Team1"))
        {
            if (this.tag.Equals("Team2")) { Destroy(this.gameObject); }
        }
        else if (otherTag.Equals("Team2"))
        {
            if (this.tag.Equals("Team1")) { Destroy(this.gameObject); }
        }
        else { Destroy(this.gameObject); }
    }
}
