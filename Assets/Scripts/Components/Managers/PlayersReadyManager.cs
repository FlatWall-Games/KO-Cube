using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayersReadyManager : NetworkBehaviour
{
    private int numPlayersReady = 0;
    private int totalPlayers = 0;
    [SerializeField] private TextMeshProUGUI countDownText;

    [ServerRpc(RequireOwnership = false)]
    public void AddPlayerServerRpc(int i)
    {
        totalPlayers+=i;
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayerReadyServerRpc()
    {
        if (++numPlayersReady == totalPlayers)
        {
            StartGameAfterCountClientRpc();
            GameObject.FindObjectOfType<AGameManager>().SetAcceptClients(false);
        }
    }

    [ClientRpc]
    private void StartGameAfterCountClientRpc()
    {
        StartCoroutine(CountDown());
    }

    IEnumerator CountDown()
    {
        GameObject.FindObjectOfType<MusicManager>().PlaySong(1, 10, false);
        countDownText.gameObject.SetActive(true);
        countDownText.text = "LA PARTIDA COMENZARÁ EN:\r\n3";
        yield return new WaitForSeconds(1);
        countDownText.text = "LA PARTIDA COMENZARÁ EN:\r\n2";
        yield return new WaitForSeconds(1);
        countDownText.text = "LA PARTIDA COMENZARÁ EN:\r\n1";
        yield return new WaitForSeconds(1);
        countDownText.text = "¡QUE COMIENCE LA PARTIDA!";
        if(IsServer) GameObject.FindObjectOfType<AGameManager>().StartGame();
        GameObject.FindObjectOfType<MusicManager>().PlaySong(2, 50, true);
        GameObject.FindObjectOfType<MusicManager>().PlayAmbient(0, true);
        yield return new WaitForSeconds(1);
        countDownText.gameObject.SetActive(false);
    }

    public void GameEnded()
    {
        StartCoroutine(DisplayEndText());
    }

    IEnumerator DisplayEndText()
    {
        countDownText.gameObject.SetActive(true);
        countDownText.text = "¡FIN DE LA PARTIDA!";
        yield return new WaitForSeconds(1.5f);
        countDownText.gameObject.SetActive(false);
    }

    public void SelectionExited()
    {
        AddPlayerServerRpc(-1);
    }
}
