using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinButton : MonoBehaviour, IPurchaseButton
{
    private static SkinButton activeButton;
    [SerializeField] private int buttonIndex = 0;
    private Skin skin;
    private Button button;
    private PurchaseWarning warning;
    private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SetActiveButton); 
        warning = PurchaseWarning.Instance;
        nameText = button.transform.Find("Text").GetComponent<TextMeshProUGUI>();
    }

    public void SetActiveButton()
    {
        warning.SetButton(this);
        if (activeButton != null) activeButton.GetComponent<Image>().color = Color.white;
        activeButton = this;
        activeButton.GetComponent<Image>().color = Color.grey;
        priceText.text = skin.GetPrice().ToString();
    }

    public void Purchase()
    {
        if (PlayerDataManager.Instance.AddCoins(-skin.GetPrice()))
        {
            MoneyText.Instance.UpdateMoney();
            Debug.Log($"Comprada la skin: {skin.GetName()}.");
        }
    }

    public string GetButtonType()
    {
        return "Skin";
    }

    public void SetSkin(Skin s)
    {
        skin = s;
        nameText.text = skin.GetName();
    }
}
