using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData
{
    public string name;
    public bool readyStatus;
}

public class NetworkConnectionManager : NetworkBehaviour
{
    public Dictionary<ulong, PlayerData> players = new Dictionary<ulong, PlayerData>();
    int numPlayersConnected;

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            RequestClientsServerRpc();
            ClientConnectionServerRpc(Singleton<PlayerDataManager>.Instance.GetName());
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        }

        base.OnNetworkSpawn();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestClientsServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        var clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        };

        foreach (var pair in players)
        {
            SendClientsClientRpc(pair.Key, pair.Value.name, pair.Value.readyStatus, clientRpcParams);
        }
    }

    [ClientRpc]
    void SendClientsClientRpc(ulong clientId, string name, bool readyStatus, ClientRpcParams rpcParams)
    {
        players.Add(clientId, new PlayerData { name = name, readyStatus = readyStatus});
    }

    [ServerRpc(RequireOwnership = false)]
    void ClientConnectionServerRpc(string name, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        NotifyConnectionClientRpc(clientId, name);
    }

    [ClientRpc]
    void NotifyConnectionClientRpc(ulong clientId, string name)
    {
        players.Add(clientId, new PlayerData() { name = name, readyStatus = false });
    }

    void OnClientDisconnect(ulong clientId)
    {
        NotifyDisconnectionClientRpc(clientId);
    }

    [ClientRpc]
    void NotifyDisconnectionClientRpc(ulong clientId)
    {
        players.Remove(clientId);
    }
}
