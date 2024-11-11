using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSkinManager : MonoBehaviour
{
    [SerializeField] private Skin[] skins;
    private int currentSkin;

    public void SetSkin(int skinIndex)
    {
        skins[currentSkin].SetActive(false);
        currentSkin = skinIndex;
        skins[currentSkin].transform.SetAsFirstSibling();
        skins[currentSkin].SetActive(true);
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}
