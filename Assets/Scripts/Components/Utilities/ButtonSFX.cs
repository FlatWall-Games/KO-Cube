using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSFX : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    private void Awake()
    {
        GetComponent<Button>()?.onClick.AddListener(OnButtonClick);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject.FindObjectOfType<SFXManager>().PlaySFX(0, 0.2f);
    }

    public void OnButtonClick()
    {
        GameObject.FindObjectOfType<SFXManager>().PlaySFX(1, 0.4f);
    }

    public void OnSelect(BaseEventData eventData)
    {
        GameObject.FindObjectOfType<SFXManager>().PlaySFX(0, 0.2f);
    }
}