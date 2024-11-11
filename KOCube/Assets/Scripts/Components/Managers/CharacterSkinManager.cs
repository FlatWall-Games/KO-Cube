using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterSkinManager : NetworkBehaviour
{
    [SerializeField] private Skin[] skins;
    private int currentSkin;

    private void Start()
    {
        if(IsOwner) SetSkinServerRpc(SkinManager.Instance.GetActiveSkin(GetComponent<CharacterInfo>().characterID));
        RequestSkinServerRpc();
    }

    [ServerRpc]
    private void SetSkinServerRpc(int skinIndex)
    {
        currentSkin = skinIndex;
    }

    [ServerRpc]
    private void RequestSkinServerRpc()
    {
        SetSkinClientRpc(currentSkin);
    }

    [ClientRpc]
    public void SetSkinClientRpc(int skinIndex)
    {
        skins[skinIndex].transform.SetAsFirstSibling();
        skins[skinIndex].SetActive(true);
        GetComponent<Animator>().Rebind();
    }
}
