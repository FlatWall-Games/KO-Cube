using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinButton : MonoBehaviour, IPurchaseButton
{
    public static SkinButton activeButton;
    public int buttonIndex = 0;
    private Skin skin;
    private Button button;
    private PurchaseWarning warning;
    private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button buyButton;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SetActiveButton); 
        nameText = button.transform.Find("Text").GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        warning = PurchaseWarning.Instance;
    }

    public void SetActiveButton()
    {
        if(warning != null) warning.SetButton(this);
        ToggleButtonStyle(); //Pone el botón activo en gris y el resto en blanco
        UpdatePriceText(); //Actualiza el texto del precio y verifica si la skin se puede comprar
        ToggleActionButton(); //Intercambia botones de comprar y de equipar
        PreviewSkinManager[] previewManagers = GameObject.FindObjectsOfType<PreviewSkinManager>();
        foreach(PreviewSkinManager pm in previewManagers) { pm.SetSkin(buttonIndex); }
    }

    private void ToggleButtonStyle()
    {
        if (activeButton != null) activeButton.GetComponent<Image>().color = Color.white;
        activeButton = this;
        activeButton.GetComponent<Image>().color = Color.grey;
    }

    private void UpdatePriceText()
    {
        if (!skin.IsAcquired()) //Si no está comprada verifica si se puede comprar
        {
            priceText.text = skin.GetPrice().ToString();
            if (skin.GetPrice() > PlayerDataManager.Instance.GetCoins()) priceText.color = Color.red;
            else priceText.color = Color.black;
            buyButton.interactable = skin.GetPrice() <= PlayerDataManager.Instance.GetCoins();
        }
        else priceText.text = "Adquirida";
        priceText.transform.Find("Coin").GetComponent<Image>().enabled = !skin.IsAcquired();
    }

    private void ToggleActionButton()
    {
        equipButton.interactable = skin.IsAcquired();
        buyButton.interactable = !skin.IsAcquired();
        if (SkinManager.Instance.GetActiveSkin(CharacterSelectionShop.Instance.index) == buttonIndex)
        {
            equipButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "EQUIPADA";
            equipButton.interactable = false;
        }
        else
        {
            equipButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "EQUIPAR";
        }
    }

    public void Purchase()
    {
        if (PlayerDataManager.Instance.AddCoins(-skin.GetPrice()))
        {
            MoneyText.Instance.UpdateMoney();
            skin.AcquireSkin(true);
            Debug.Log($"Comprada la skin: {skin.GetName()}.");
            SetActiveButton();
            SkinManager.Instance.UpdateData(CharacterSelectionShop.Instance.index);
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
