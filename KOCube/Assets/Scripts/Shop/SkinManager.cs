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
    }

    public int GetActiveSkin(int characterID)
    {
        return activeSkins[characterID];
    }
}
