using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSFX : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject.FindObjectOfType<SFXManager>().PlaySFX(0, 0.2f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject.FindObjectOfType<SFXManager>().PlaySFX(1, 0.4f);
    }
}