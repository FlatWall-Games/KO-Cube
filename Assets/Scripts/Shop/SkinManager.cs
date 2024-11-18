using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance;
    [SerializeField] private Skin[] MaxinganSkins;
    [SerializeField] private Skin[] JudySkins;
    [SerializeField] private Skin[] BaleSkins;
    [SerializeField] private Skin[] CleoSkins;
    [SerializeField] private Skin[] AlraSkins;
    [SerializeField] private Skin[] SadGuySkins;
    private List<Skin[]> totalSkins = new List<Skin[]>();

    //Lista de skins activas:
    private int[] activeSkins = new int[6];

    //Lista de previews:
    [SerializeField] private GameObject[] characterPreviews;

    private void Awake()
    {
        Instance = this;
        totalSkins.Add(MaxinganSkins);
        totalSkins.Add(JudySkins);
        totalSkins.Add(BaleSkins);
        totalSkins.Add(CleoSkins);
        totalSkins.Add(AlraSkins);
        totalSkins.Add(SadGuySkins);
    }

    public Skin[] GetCharacterSkins(int characterID)
    {
        return totalSkins[characterID];
    }

    public void SetActiveSkin(int characterID, int activeSkin)
    {
        activeSkins[characterID] = activeSkin;
        Debug.Log($"Equipada la skin número {activeSkin} para el personaje con ID: {characterID}");
        PlayerDataManager.Instance.SetActiveSkins(activeSkins);
    }

    public int GetActiveSkin(int characterID)
    {
        return activeSkins[characterID];
    }

    public void RestoreData(int characterID, List<bool> boolList)
    {
        for(int i = 0; i < boolList.Count; i++)
        {
            totalSkins[characterID][i].AcquireSkin(boolList[i]);
        }
    }

    public void RestoreActiveSkins(int[] skins)
    {
        activeSkins = skins;
        RestorePreviews();
    }

    public void UpdateData(int index)
    {
        List<bool> list = new List<bool>();
        foreach(Skin skin in totalSkins[index])
        {
            list.Add(skin.IsAcquired());
        }
        PlayerDataManager.Instance.SetSkins(index, list);
    }

    private void RestorePreviews() 
    {
        for (int i = 0; i < characterPreviews.Length; i++)
        {
            foreach(PreviewSkinManager psm in characterPreviews[i].GetComponents<PreviewSkinManager>())
            {
                psm.SetSkin(activeSkins[i]);
            }
            if (i != 0) characterPreviews[i].SetActive(false);
        }
    }
}
