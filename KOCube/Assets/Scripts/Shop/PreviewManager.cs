using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewManager : MonoBehaviour
{
    [SerializeField] private GameObject[] characterPreviews;
    private int index = 0;

    public int ChangePreview(int i)
    {
        characterPreviews[index].SetActive(false);
        index += i;
        if (index < 0) index = characterPreviews.Length - 1;
        else if (index == characterPreviews.Length) index = 0;
        characterPreviews[index].SetActive(true);
        return index;
    }
    
    public void ForcePreview(int i) //Fuerza la muestra de un elemento
    {
        characterPreviews[index].SetActive(false);
        index = i;
        characterPreviews[index].SetActive(true);
    }
}
