using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CleoProjectile : AProjectile
{
    CleoPassive passive;
    private bool hit = false;
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if(attacker != null) passive = attacker.GetComponent<CleoPassive>();
    }

    public override void CheckDestroy(Collider other) //Cada proyectil tiene sus condiciones de destrucción
    {
        if (this.tag.Equals("Team1") && other.tag.Equals("Team2")) hit = true;
        else if (this.tag.Equals("Team2") && other.tag.Equals("Team1")) hit = true;
    }

    private void OnDestroy()
    {
        if (passive != null)
        {
            if (hit) passive.AddHit();
            else passive.ResetCounter();
        }
    }
}
