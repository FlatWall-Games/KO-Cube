using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using System.Runtime.InteropServices.WindowsRuntime;

public class UIManager : MonoBehaviour, IUI
{
    const int maxConnections = 5; //Número máximo de conexiones de cada cliente sin contarle.

    //Texto por defecto para las áreas de texto en las que introducir el código de sala y el nombre del jugador.
    string joinCode = "Enter room code...";
    public string playerName = "Enter player name...";

    private IState _currentState;
    private GameObject _canvas;

    public IState State 
    { 
        get { return _currentState; } 
        set 
        {
            if (_currentState != null) _currentState.Exit();
            _currentState = value;
            _currentState.Enter();
        } 
    }
    public GameObject Canvas { get { return _canvas; } set { _canvas = value; } }

    void Start()
    {
        Canvas = GameObject.Find("Canvas");
        State = new StartMenuState(this);
    }

    void Update()
    {
        State.Update();
    }

    void FixedUpdate()
    {
        State.FixedUpdate();
    }

    //Método encargado de inicializar un runtime como host de una partida del juego.
    async void StartHost()
    {

        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "wss"));
        joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        NetworkManager.Singleton.StartHost();

    }

    //Método encargado de inicializar un runtime como cliente de una partida de juego que tiene de código "joinCode".
    async void StartClient(string joinCode)
    {

        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "wss"));

        NetworkManager.Singleton.StartClient();

    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
        }

        GUILayout.EndArea();
    }

    void StartButtons()
    {
        if (GUILayout.Button("Host")) StartHost();
        if (GUILayout.Button("Client")) StartClient(joinCode);
        playerName = GUILayout.TextArea(playerName);
        joinCode = GUILayout.TextArea(joinCode);
    }

    void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
            NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
        GUILayout.Label("Room: " + joinCode);
    }
}
