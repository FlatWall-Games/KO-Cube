using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldColor : MonoBehaviour
{
    [SerializeField] private Renderer shieldRenderer;
    [SerializeField] private Material redMaterial;

    public void UpdateColor(string tag)
    {
        if (!PlayerBehaviour.ownerTag.Equals(tag))
        {
            shieldRenderer.material = redMaterial;
            shieldRenderer.materials[1] = new Material(shieldRenderer.materials[1]);
            shieldRenderer.materials[1].SetColor("_color", Color.red);
        }
    }
}
