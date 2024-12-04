using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaleProjectile : AProjectile
{
    [Header("EFFECTS:")]
    [SerializeField] private GameObject flashPrefab;

    protected override void Awake()
    {
        base.Awake();
        rb.velocity = this.transform.forward * speed;
        this.transform.parent = null; //Se desvincula del padre para que no le afecte su movimiento
    }

    public override void CheckDestroy(Collider other) //Cada proyectil tiene sus condiciones de destrucci�n
    {
        string otherTag = other.tag;
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

    public override void SetAttacker(PlayerBehaviour attacker)
    {
        base.SetAttacker(attacker);
        GameObject flash = GameObject.Instantiate(flashPrefab, attacker.gameObject.transform.Find("BasicOrigin"));
        Destroy(flash, 0.5f);
    }
}
