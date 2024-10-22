using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudyniUlt : AProjectile
{
    protected override void Awake()
    {
        base.Awake();
        Transform parent = transform.parent;
        rb.velocity = this.transform.forward * speed;
        this.transform.parent = null; //Se desvincula del padre para que no le afecte su movimiento
        
    }

    public override void CheckDestroy(Collider other) //Cada proyectil tiene sus condiciones de destrucción
    {
        string otherTag = other.tag;

        //this.attacker.SetPosition(this.transform.position);
        //public void SetPosition(Vector3 pos)
        //{
        //    if (!IsServer) return;
        //    Debug.Log($"TP hacia {pos} desde {this.transform.position}");
        //    this.transform.position = pos;
        //}

        //En este caso, el proyectil se destruye al chocar con un jugador del otro equipo o con un objeto del mapa
        if (otherTag.Equals("Team1"))
        {
            if (this.tag.Equals("Team2"))
            {
                Destroy(this.gameObject);
            }
        }
        else if (otherTag.Equals("Team2"))
        {
            if (this.tag.Equals("Team1")) 
            { 
                Destroy(this.gameObject);
            }
        }
        else 
        {
            
            Destroy(this.gameObject); 
        }
    }
        
    
}
