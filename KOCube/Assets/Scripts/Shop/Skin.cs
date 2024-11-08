using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skin : MonoBehaviour
{
    [SerializeField] private Material[] materials; //Materiales que tendrá la skin
    [SerializeField] private int price;
    [SerializeField] private string skinName;

    public string GetName()
    {
        return skinName;
    }

    public Material[] GetMaterials()
    {
        return materials;
    }

    public int GetPrice()
    {
        return price;
    }
}
