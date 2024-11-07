using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopTypeButton : MonoBehaviour
{
    public bool active = false;

    private void Awake()
    {
        ToggleStyle();
    }

    public void ToggleButton()
    {
        active = !active;
        ToggleStyle();
    }

    private void ToggleStyle()
    {
        Color originalColor = GetComponent<Image>().color;
        if (active)
        {
            GetComponent<Image>().color = new Color(originalColor.r, originalColor.g, originalColor.b, 1);
        }
        else
        {
            GetComponent<Image>().color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.2f);
        }
    }
}
