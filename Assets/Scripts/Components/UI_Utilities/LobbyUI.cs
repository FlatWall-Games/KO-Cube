using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numPlayersLobbyText;
    [SerializeField] private TextMeshProUGUI playersReadyText;
    [SerializeField] private TextMeshProUGUI joinCodeText;
    [SerializeField] private List<GameObject> lobbyPlayers;

    // Start is called before the first frame update
    void Start()
    {
        joinCodeText.text = "Código: " + Singleton<UIManager>.Instance.JoinCode.ToString();

        int i = 0;
        foreach (PlayerData playerData in GameObject.FindAnyObjectByType<NetworkConnectionManager>().players.Values)
        {
            lobbyPlayers[i].transform.Find($"PlayerName{i + 1}").GetComponent<TextMeshProUGUI>().text = playerData.name;
            lobbyPlayers[i].transform.Find($"PlayerStatus{i + 1}").GetComponentInChildren<TextMeshProUGUI>().text = playerData.readyStatus? "Listo" : "No listo";
            i++;
        }

        while ( i < lobbyPlayers.Count)
        {
            lobbyPlayers[i].transform.Find($"PlayerName{i + 1}").GetComponent<TextMeshProUGUI>().text = "";
            lobbyPlayers[i].transform.Find($"PlayerStatus{i + 1}").gameObject.SetActive(false);
            lobbyPlayers[i].transform.Find($"PlayerStatus{i + 1}").GetComponentInChildren<TextMeshProUGUI>().text = "";
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        numPlayersLobbyText.text = GameObject.FindAnyObjectByType<NetworkConnectionManager>().players.Count.ToString() + " / 6"; 
        playersReadyText.text = GameObject.FindObjectOfType<PlayersReadyManager>().NumPlayersReady.ToString() + " / " + GameObject.FindAnyObjectByType<NetworkConnectionManager>().players.Count.ToString();

        int i = 0;
        foreach (PlayerData playerData in GameObject.FindAnyObjectByType<NetworkConnectionManager>().players.Values)
        {
            lobbyPlayers[i].transform.Find($"PlayerName{i + 1}").GetComponent<TextMeshProUGUI>().text = playerData.name;
            lobbyPlayers[i].transform.Find($"PlayerStatus{i + 1}").gameObject.SetActive(true);
            lobbyPlayers[i].transform.Find($"PlayerStatus{i + 1}").GetComponentInChildren<TextMeshProUGUI>().text = playerData.readyStatus ? "Listo" : "No listo";
            i++;
        }
    }
}
