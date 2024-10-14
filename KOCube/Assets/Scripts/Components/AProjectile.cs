using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AProjectile : MonoBehaviour, IAttack
{
    [SerializeField] protected float speed; //Velocidad de la bala
    [SerializeField] protected float timeToDestroy; //Tiempo que tarda en destruirse
    [SerializeField] protected float damage; //Daño que hace a enemigos
    [SerializeField] protected float healing; //Vida que cura a aliados
    protected PlayerMovement attacker; //Jugador que lanzó el ataque
    protected Rigidbody rb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(this.gameObject, timeToDestroy); //Se programa su destrucción
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckDestroy(other.tag);
    }

    public virtual void CheckDestroy(string otherTag) { }

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
