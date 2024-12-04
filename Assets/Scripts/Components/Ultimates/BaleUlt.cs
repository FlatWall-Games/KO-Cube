using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaleUlt : AProjectile
{
    [Header("EFFECTS: ")]
    [SerializeField] private GameObject flashPrefab;

    protected override void Awake()
    {
        base.Awake();
        rb.velocity = this.transform.forward * speed;
        this.transform.parent = null; //Se desvincula del padre para que no le afecte su movimiento
    }

    public override void CheckDestroy(Collider other) //Cada proyectil tiene sus condiciones de destrucción
    {
        string otherTag = other.tag;
        if (otherTag.Equals(this.tag))
            return;
        this.damage += 10; 
    }

    public override void SetAttacker(PlayerBehaviour attacker)
    {
        base.SetAttacker(attacker);
        GameObject flash = GameObject.Instantiate(flashPrefab, attacker.gameObject.transform.Find("UltOrigin"));
        Destroy(flash, 0.5f);
    }
}
