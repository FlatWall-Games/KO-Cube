using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachinganUlt : AProjectile
{
    protected override void Awake()
    {
        base.Awake();
        rb.velocity = this.transform.forward * speed;
        this.transform.parent = null; //Se desvincula del padre para que no le afecte su movimiento
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Floor")) return;
        GetComponent<Rigidbody>().isKinematic = true;
        tag = "Untagged";
    }

    public override void CheckDestroy(string otherTag) //Cada proyectil tiene sus condiciones de destrucción
    {
        
    }
}
