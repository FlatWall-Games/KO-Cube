using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class JudyProjectile : AProjectile
{
    protected override void Awake()
    {
        //if (!NetworkManager.Singleton.IsServer) return;

        base.Awake();
    Transform parent = transform.parent;
    }
}
