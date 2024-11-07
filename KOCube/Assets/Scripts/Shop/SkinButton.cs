using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinButton : MonoBehaviour, IPurchaseButton
{
    //Debería tener una variable Skin
    private Button button;
    private PurchaseWarning warning;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(GoToPurchaseWarning); //Me daba pereza asignarlo desde el inspector
        warning = PurchaseWarning.Instance;
    }

    private void GoToPurchaseWarning()
    {
        warning.SetButton(this);
        warning.gameObject.SetActive(true);
    }

    public void Purchase()
    {
        string skinName = button.transform.Find("Text").GetComponent<TextMeshProUGUI>().text;
        Debug.Log($"Comprada la skin: {skinName}.");
    }

    public string GetButtonType()
    {
        return "Skin";
    }
}
