using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ShieldColor : NetworkBehaviour
{
    [SerializeField] private Renderer shieldRenderer;
    [SerializeField] private Material redMaterial;

    void Start()
    {
        if (IsServer) UpdateColorClientRpc(this.tag);
    }

    [ClientRpc]
    private void UpdateColorClientRpc(string tag)
    {
        if (!PlayerBehaviour.ownerTag.Equals(tag)) shieldRenderer.material = redMaterial;
    }
}
