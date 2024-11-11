using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skin : MonoBehaviour
{
    [SerializeField] private GameObject[] skinObjects; 
    [SerializeField] private int price;
    [SerializeField] private string skinName;
    [SerializeField] private bool acquired = false;

    public void SetActive(bool active)
    {
        foreach(GameObject obj in skinObjects)
        {
            obj.SetActive(active);
        }
    }

    public string GetName()
    {
        return skinName;
    }

    public GameObject[] GetObjects()
    {
        return skinObjects;
    }

    public int GetPrice()
    {
        return price;
    }

    public bool IsAcquired()
    {
        return acquired;
    }

    public void AcquireSkin(bool acquire)
    {
        acquired = acquire;
    }
}
