using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ModeSelector : NetworkBehaviour
{
    private int currentModeIndex = 0;
    [SerializeField] private GameObject[] modes;

    [ServerRpc(RequireOwnership = false)]
    public void RequestModeServerRpc()
    {
        AssignModeClientRpc(currentModeIndex);
    }

    [ClientRpc]
    private void AssignModeClientRpc(int index)
    {
        currentModeIndex = index;
        UpdateMode();
    }

    public void SetMode(int index) //Llamado desde los botones que sólo el host puede ver en la pestaña de elección 
    {
        currentModeIndex = index;
    }

    public void UpdateMode() //Llamada desde los clientes cuando se les solicita modo de juego y desde el host cuando lo selecciona
    {
        for(int i = 0; i < modes.Length; i++)
        {
            if (i != currentModeIndex) modes[i].SetActive(false);
        }
        GameObject.FindObjectOfType<AGameManager>().SyncSpectatorData();
    }
}
