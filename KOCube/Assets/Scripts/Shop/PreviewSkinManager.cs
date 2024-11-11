using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSkinManager : MonoBehaviour
{
    [SerializeField] private Skin[] skins;
    [SerializeField] private string rootName;
    private int currentSkin;

    public void SetSkin(int skinIndex)
    {
        skins[currentSkin].SetActive(false);
        skins[currentSkin].name = "a";
        currentSkin = skinIndex;
        skins[currentSkin].name = rootName;
        skins[currentSkin].transform.SetAsFirstSibling();
        skins[currentSkin].SetActive(true);
        GetComponent<Animator>().Rebind();
    }
}
