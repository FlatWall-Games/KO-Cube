using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : MonoBehaviour, IAttack
{
    [SerializeField] private float speed; //Velocidad de la bala
    [SerializeField] private float timeToDestroy; //Tiempo que tarda en destruirse
    [SerializeField] private float damage; //Daño que hace a enemigos
    [SerializeField] private float healing; //Vida que cura a aliados
    private PlayerMovement attacker; //Jugador que lanzó el ataque
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = this.transform.forward * speed;
        this.transform.parent = null; //Se desvincula del padre para que no le afecte su movimiento
        Destroy(this.gameObject, timeToDestroy); //Se programa su destrucción
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckDestroy(other.tag);
    }

    public void CheckDestroy(string otherTag) //Cada proyectil tiene sus condiciones de destrucción
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

    public float GetDamage()
    {
        return damage;
    }

    public float GetHealing()
    {
        return healing;
    }

    public string GetTag()
    {
        return this.tag;
    }

    public void SetTag(string tag)
    {
        this.tag = tag;
    }

    public void SetAttacker(PlayerMovement attacker)
    {
        this.attacker = attacker;
    }

    public PlayerMovement GetAttacker()
    {
        return attacker;
    }
}
