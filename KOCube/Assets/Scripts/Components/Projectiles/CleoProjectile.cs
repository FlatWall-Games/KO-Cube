using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CleoProjectile : AProjectile
{
    CleoThreeHitsBehaviour cleoThreeHits;
    protected override void Awake()
    {
        base.Awake();
        Transform parent = transform.parent;
    }

    public override void CheckDestroy(Collider other) //Cada proyectil tiene sus condiciones de destrucci�n
    {
        string otherTag = other.tag;
        cleoThreeHits = this.attacker.GetComponentInChildren<CleoThreeHitsBehaviour>();
        if (cleoThreeHits.GetHits() >= 2)
        {
            GameObject.Instantiate(this, transform.parent);
            cleoThreeHits.ResetHits();
        }
        else
        {
            //En este caso, el proyectil se destruye al chocar con un jugador del otro equipo o con un objeto del mapa
            if (otherTag.Equals("Team1"))
            {
                if (this.tag.Equals("Team2"))
                {
                    cleoThreeHits.AddHit();
                }
            }
            else if (otherTag.Equals("Team2"))
            {
                if (this.tag.Equals("Team1"))
                {
                    cleoThreeHits.AddHit();
                }
            }
            else
            {
                cleoThreeHits.ResetHits();
            }
        }
        
    }
}