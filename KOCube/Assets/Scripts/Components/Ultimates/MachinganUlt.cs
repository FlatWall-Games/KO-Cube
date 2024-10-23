using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MachinganUlt : AProjectile
{
    protected override void Awake()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        base.Awake();
        rb.velocity = this.transform.forward * speed;
        this.transform.parent = null; //Se desvincula del padre para que no le afecte su movimiento
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (!collision.gameObject.CompareTag("Floor")) return;
        GetComponent<Rigidbody>().isKinematic = true;
        tag = "Untagged";
    }

    public override void CheckDestroy(Collider other) //Cada proyectil tiene sus condiciones de destrucción
    {
        
    }
}
