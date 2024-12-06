using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playersReadyText;
    [SerializeField] private TextMeshProUGUI joinCodeText;

    // Start is called before the first frame update
    void Start()
    {
        joinCodeText.text = "Código: " + Singleton<UIManager>.Instance.JoinCode.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        playersReadyText.text = GameObject.FindObjectOfType<PlayersReadyManager>().NumPlayersReady.ToString() + " / " + NetworkManager.Singleton.ConnectedClients.Count.ToString();
    }
}
