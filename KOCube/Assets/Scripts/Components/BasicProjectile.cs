using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : MonoBehaviour, IAttack
{
    [SerializeField] private float speed;
    [SerializeField] private float timeToDestroy;
    [SerializeField] private float damage;
    [SerializeField] private float healing;
    private PlayerMovement attacker;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = this.transform.forward * speed;
        this.transform.parent = null;
        Destroy(this.gameObject, timeToDestroy);
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckDestroy(other.tag);
    }

    public void CheckDestroy(string otherTag)
    {
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
