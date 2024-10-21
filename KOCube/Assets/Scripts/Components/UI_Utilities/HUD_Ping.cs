using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class HUD_Ping : NetworkBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;
    float sendedTime;
    float responseTime;
    float requestTimer;
    int pingValue = 0;
    bool canStartWorking = false;

    private void Update()
    {
        if (!canStartWorking) return;
        if (IsServer) return;

        if (requestTimer >= 1f) 
        {
            requestTimer = 0;
            ClientPingRequest();
        }
        else
        {
            requestTimer += Time.deltaTime;
        }
        
    }

    private void ClientPingRequest()
    {
        sendedTime = Time.realtimeSinceStartup;
        SendMessageServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendMessageServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;

        RespondToClientRpc(clientId);
    }

    [ClientRpc]
    public void RespondToClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            responseTime = Time.realtimeSinceStartup;
            pingValue = (int)(responseTime - sendedTime) * 1000;
            textMeshProUGUI.text = "Ping: " + pingValue + "ms";
        }
    }

    public void CanStartWorking()
    {
        canStartWorking = true;
    }
}
