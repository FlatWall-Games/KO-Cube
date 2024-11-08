using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionShop : MonoBehaviour
{
    public static CharacterSelectionShop Instance;
    [SerializeField] private Sprite[] charactersImages;
    [SerializeField] private Image currentCharacterImage;
    [SerializeField] private SkinButton[] buttons;
    public int index = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        Invoke("UpdateSkinButtons", 0.01f); //Sin este pequeño delay no funciona la primera vez que abres la tienda, por la cara
    }

    public void ChangeImage(int i)
    {
        index += i;
        if (index < 0) index = charactersImages.Length-1;
        else if (index == charactersImages.Length) index = 0;
        currentCharacterImage.sprite = charactersImages[index];
        UpdateSkinButtons();
    }

    private void UpdateSkinButtons()
    {
        Skin[] skins = SkinManager.Instance.GetCharacterSkins(index);
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetSkin(skins[i]);
        }
        buttons[SkinManager.Instance.GetActiveSkin(index)].SetActiveButton();
    }

    public void SetActiveSkin()
    {
        SkinManager.Instance.SetActiveSkin(index, SkinButton.activeButton.buttonIndex);
        SkinButton.activeButton.SetActiveButton();
    }
}
