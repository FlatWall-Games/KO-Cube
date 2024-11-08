using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skin : MonoBehaviour
{
    [SerializeField] private Material[] materials; //Materiales que tendrá la skin
    [SerializeField] private int price;
    [SerializeField] private string skinName;
    [SerializeField] private bool acquired = false;

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

    public bool IsAcquired()
    {
        return acquired;
    }

    public void AcquireSkin()
    {
        acquired = true;
    }
}
