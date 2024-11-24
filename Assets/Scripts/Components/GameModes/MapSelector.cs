using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MapSelector : NetworkBehaviour
{
    private int currentMapIndex = 0;
    [SerializeField] private GameObject[] maps;

    [ServerRpc(RequireOwnership = false)]
    public void RequestMapServerRpc()
    {
        AssignMapClientRpc(currentMapIndex);
    }

    [ClientRpc]
    private void AssignMapClientRpc(int index)
    {
        currentMapIndex = index;
        UpdateMap();
    }

    public void SetMap(int index) //Llamado desde los botones que sólo el host puede ver en la pestaña de elección 
    {
        currentMapIndex = index;
    }

    public void UpdateMap() //Llamada desde los clientes cuando se les solicita modo de juego y desde el host cuando lo selecciona
    {
        for (int i = 0; i < maps.Length; i++)
        {
            if (i != currentMapIndex) maps[i].SetActive(false);
            else maps[i].transform.Find("Map").gameObject.SetActive(true);
        }
    }
}

