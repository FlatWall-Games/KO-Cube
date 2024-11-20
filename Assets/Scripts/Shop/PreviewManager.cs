using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewManager : MonoBehaviour
{
    [SerializeField] private GameObject[] characterPreviews;
    public event Action OnPreviewChanged;
    private int index = 0;

    public int ChangePreview(int i)
    {
        characterPreviews[index].SetActive(false);
        RestorePreviewRotation();
        index += i;
        if (index < 0) index = characterPreviews.Length - 1;
        else if (index == characterPreviews.Length) index = 0;
        characterPreviews[index].SetActive(true);
        OnPreviewChanged?.Invoke();
        return index;
    }
    
    public void ForcePreview(int i) //Fuerza la muestra de un elemento
    {
        characterPreviews[index].SetActive(false);
        index = i;
        characterPreviews[index].SetActive(true);
    }

    public GameObject GetCurrentPreview()
    {
        return characterPreviews[index];
    }

    public void RestorePreviewRotation()
    {
        characterPreviews[index].transform.rotation = Quaternion.Euler(0, 203, 0);
    }
}
