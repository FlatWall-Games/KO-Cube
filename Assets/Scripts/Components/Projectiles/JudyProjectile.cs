using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudyProjectile : AProjectile
{
    protected override void Awake()
    {
    base.Awake();
    Transform parent = transform.parent;
    }
}
