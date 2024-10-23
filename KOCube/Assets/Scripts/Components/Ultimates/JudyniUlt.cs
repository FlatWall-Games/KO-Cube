using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class JudyniUlt : AProjectile
{
    [SerializeField]Transform tpPoint;
    [SerializeField] bool activateTp = false;
    protected override void Awake()
    {
        //if (!NetworkManager.Singleton.IsServer) return;

        base.Awake();
        Transform parent = transform.parent;
        rb.velocity = this.transform.forward * speed;
        this.transform.parent = null; //Se desvincula del padre para que no le afecte su movimiento
        
    }
    private void FixedUpdate()
    {
       // if (!NetworkManager.Singleton.IsServer) return;

        if (activateTp)
        {
            Debug.Log($"TP hacia {transform.position} desde {tpPoint.position}");
            this.attacker.GetComponent<CharacterController>().enabled = false;
            this.attacker.transform.position = tpPoint.position;
            this.attacker.GetComponent<CharacterController>().enabled = true;
            activateTp = false;
        }
    }
    public override void CheckDestroy(Collider other) //Cada proyectil tiene sus condiciones de destrucción
    {
        //if (!NetworkManager.Singleton.IsServer) return;

        string otherTag = other.tag;
        if (other != this.attacker)
        {
            activateTp = true;
        }
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
