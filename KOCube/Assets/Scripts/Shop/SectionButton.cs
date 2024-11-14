using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SectionButton : MonoBehaviour
{
    public bool active = false;
    [SerializeField] private SectionButton[] otherButtons;
    public GameObject[] sections;
    private int sectionIndex = 0;

    private void Awake()
    {
        //ToggleStyle();
        if (!active) sections[sectionIndex].SetActive(false);
        else GetComponent<Button>().interactable = false;
    }

    public void ToggleButton()
    {
        active = !active;
        sections[sectionIndex].SetActive(active);
        GetComponent<Button>().interactable = !GetComponent<Button>().interactable;
        //ToggleStyle();
    }

    public void TurnOffOtherButtons()
    {
        foreach(SectionButton button in otherButtons)
        {
            button.active = false;
            button.GetComponent<Button>().interactable = true;
            //button.ToggleStyle();
            button.sections[sectionIndex].SetActive(false);
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

    public void SetSection(int index)
    {
        sections[sectionIndex].SetActive(false);
        sectionIndex = index;
        if(active) sections[sectionIndex].SetActive(true);
    }
}
