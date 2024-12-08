using Unity.Netcode;
using UnityEngine;

public class DeathMatchManager : AGameManager
{
    protected override void Awake()
    {
        base.Awake();
        gameModeUiText.text = $"Elimina a {maxPoints} enemigos para ganar!";
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void InvertUI()
    {
        if (inverted) return;
        base.InvertUI();
    }

    [ClientRpc]
    public void DisplayKillInfoClientRpc(string killerName, string killedName, string killerTag)
    {
        string info;
        if (PlayerBehaviour.ownerTag.Equals(killerTag)) info = $"<color=#00FFFF>{killerName}</color> ha eliminado a <color=red>{killedName}";
        else info = $"<color=red>{killerName}</color> ha eliminado a <color=cyan>{killedName}";
        DisplayInformation(info, 2);
    }
}