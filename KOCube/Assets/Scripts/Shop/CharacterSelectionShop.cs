using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionShop : MonoBehaviour
{
    [SerializeField] private Sprite[] charactersImages;
    [SerializeField] private Image currentCharacterImage;
    [SerializeField] private SkinButton[] buttons;
    int index = 0;

    private void OnEnable()
    {
        UpdateSkinButtons();
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
        buttons[0].SetActiveButton();
    }
}
