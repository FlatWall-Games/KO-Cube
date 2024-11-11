using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkinManager : MonoBehaviour
{
    [SerializeField] private Skin[] skins;
    [SerializeField] private bool playMode = false;
    private int currentSkin;

    public void SetSkin(int skinIndex)
    {
        skins[currentSkin].SetActive(false);
        currentSkin = skinIndex;
        skins[currentSkin].transform.SetAsFirstSibling();
        skins[currentSkin].SetActive(true);
        if(playMode)
        {
            GetComponent<Animator>().enabled = true;
            return;
        }
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}
