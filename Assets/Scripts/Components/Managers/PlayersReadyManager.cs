using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;

public class PlayersReadyManager : NetworkBehaviour
{
    private int numPlayersReady = 0;
    [SerializeField] private TextMeshProUGUI countDownText;

    public int NumPlayersReady
    {
        get
        {
            return numPlayersReady;
        }
    }

    [ServerRpc]
    public void PlayerReadyServerRpc(bool ready, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        NotifyClientStatusClientRpc(clientId, ready);

        if (ready) { numPlayersReady++;} else { numPlayersReady--; }

        if (numPlayersReady == NetworkManager.Singleton.ConnectedClients.Count)
        {
            StartGameAfterCountClientRpc();
            GameObject.FindObjectOfType<AGameManager>().SetAcceptClients(false);
        }
    }

    [ClientRpc]
    void NotifyClientStatusClientRpc(ulong clientId, bool ready)
    {
        var playerData = GameObject.FindAnyObjectByType<NetworkConnectionManager>().players[clientId];
        playerData.readyStatus = ready;
        GameObject.FindAnyObjectByType<NetworkConnectionManager>().players[clientId] = playerData;
    }

    [ClientRpc]
    private void StartGameAfterCountClientRpc()
    {
        GameObject.FindObjectOfType<AGameManager>().transform.Find("Canvas/LobbyUI").gameObject.SetActive(false);
        StartCoroutine(CountDown());
    }

    IEnumerator CountDown()
    {
        GameObject.FindObjectOfType<MusicManager>().PlaySong(1, 10, false);
        countDownText.gameObject.SetActive(true);
        countDownText.text = "LA PARTIDA COMENZARA EN:\r\n3";
        yield return new WaitForSeconds(1);
        countDownText.text = "LA PARTIDA COMENZARA EN:\r\n2";
        yield return new WaitForSeconds(1);
        countDownText.text = "LA PARTIDA COMENZARA EN:\r\n1";
        yield return new WaitForSeconds(1);
        countDownText.text = "QUE COMIENCE LA PARTIDA!";
        if(IsServer) GameObject.FindObjectOfType<AGameManager>().StartGame();
        GameObject.FindObjectOfType<MusicManager>().PlaySong(2, 50, true);
        GameObject.FindObjectOfType<MusicManager>().PlayAmbient(0, true);
        yield return new WaitForSeconds(1);
        countDownText.gameObject.SetActive(false);
    }

    public void GameEnded()
    {
        GameObject.FindObjectOfType<PreviewManager>().UpdateTeammatesPreview();
        StartCoroutine(DisplayEndText());
    }

    IEnumerator DisplayEndText()
    {
        countDownText.gameObject.SetActive(true);
        countDownText.text = "FIN DE LA PARTIDA!";
        yield return new WaitForSeconds(1.5f);
        countDownText.gameObject.SetActive(false);
    }
}
