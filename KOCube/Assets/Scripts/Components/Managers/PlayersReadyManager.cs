using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayersReadyManager : NetworkBehaviour
{
    private int numPlayersReady = 0;
    private int totalPlayers = 0;

    [ServerRpc(RequireOwnership = false)]
    public void AddPlayerServerRpc()
    {
        totalPlayers++;
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayerReadyServerRpc()
    {
        if(++numPlayersReady == totalPlayers) GameObject.FindObjectOfType<AGameManager>().StartGame();
    }
}
