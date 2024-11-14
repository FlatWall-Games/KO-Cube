using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SectionButton : MonoBehaviour
{
    public bool active = false;
    [SerializeField] private SectionButton[] otherButtons;
    public GameObject section;

    private void Awake()
    {
        ToggleStyle();
        if (!active) section.SetActive(false);
    }

    public void ToggleButton()
    {
        active = !active;
        section.SetActive(active);
        GetComponent<Button>().interactable = !GetComponent<Button>().interactable;
        ToggleStyle();
    }

    public void TurnOffOtherButtons()
    {
        foreach(SectionButton button in otherButtons)
        {
            button.active = false;
            button.GetComponent<Button>().interactable = true;
            //button.ToggleStyle();
            button.section.SetActive(false);
        }
    }

    public void ToggleStyle()
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

    public void ForceEnable()
    {
        if (!active)
        {
            ToggleButton();
            TurnOffOtherButtons();
        }
    }
}
