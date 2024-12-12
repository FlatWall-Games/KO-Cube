using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterSkinManager : NetworkBehaviour
{
    [SerializeField] private Skin[] skins;
    [SerializeField] private string rootName;
    private int currentSkin = -1;

    private void Start()
    {
        if (IsOwner) SetSkinServerRpc(SkinManager.Instance.GetActiveSkin(GetComponent<CharacterInfo>().characterID));
        else RequestSkinServerRpc();
    }

    [ServerRpc]
    private void SetSkinServerRpc(int skinIndex)
    {
        currentSkin = skinIndex;
        RequestSkinServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSkinServerRpc()
    {
        if (currentSkin == -1) return; //Si todavía no ha sido asignada hace return
        SetSkinClientRpc(currentSkin);
    }

    [ClientRpc]
    public void SetSkinClientRpc(int skinIndex)
    {
        currentSkin = skinIndex;
        skins[skinIndex].transform.SetAsFirstSibling();
        skins[skinIndex].name = rootName;
        skins[skinIndex].SetActive(true);
        GetComponent<Animator>().Rebind();
    }

    public int GetSkin()
    {
        return currentSkin;
    }
}
