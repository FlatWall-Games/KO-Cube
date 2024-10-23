using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SadGuyProjectile : AProjectile
{
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] float upwardForce = 2.5f;
    Vector3 characterVelocity;

    protected override void Awake()
    {
        base.Awake();
        characterVelocity = transform.parent.parent.GetComponent<CharacterController>().velocity;
        rb.velocity = this.transform.forward * speed + Vector3.up * upwardForce;
        this.transform.parent = null; //Se desvincula del padre para que no le afecte su movimiento
    }

    public override void CheckDestroy(Collider other) //Cada proyectil tiene sus condiciones de destrucción
    {
        PlayerBehaviour receiver = other.GetComponent<PlayerBehaviour>();
        if (receiver != GetAttacker())
        {
            IAttack projectile = GameObject.Instantiate(explosionPrefab, transform.position, transform.rotation).GetComponent<IAttack>();
            projectile.SetTag(this.tag); //Le pone tag para que gestione colisiones, daño y curas
            projectile.SetAttacker(GetAttacker()); //Se configura para que sepa quién lanzó el ataque

            Destroy(this.gameObject);
        }
    }
}
