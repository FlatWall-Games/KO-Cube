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
using TMPro;
using System;
using System.Net.Http;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>, IUI
{
    const int _maxConnections = 5; //Número máximo de conexiones de cada lobby sin contar al host.

    string _joinCode; //Código de la sala.

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
        //Suscribimos un método que se encargará de que se cambie el estado correctamente cuando se cargue una nueva escena
        //SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(Canvas);
    }

    void Update()
    {
        State.Update();
    }

    void FixedUpdate()
    {
        State.FixedUpdate();
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        GUI.skin.label.fontSize = 25;
        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer)
        {
            StatusLabels();
        }
        GUILayout.EndArea();
    }

    void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
            NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
        GUILayout.Label("Room: " + _joinCode);
    }

    public void StartGame()
    {
        Debug.Log(PlayerDataManager.Instance.GetName());
        if (PlayerDataManager.Instance.GetName() != null)
        {
            GoMainMenu();
        }
        else
        {
            State = new UserSetupState(this);
        }
    }

    public void Play()
    {
        State = new LobbyHostJoinState(this);
    }

    public void GoMainMenu()
    {
        State = new MainMenuState(this);
    }

    public void StartGameAsHost()
    {
        StartHost();
    }

    //Método encargado de inicializar un runtime como host de una partida del juego.
    async void StartHost()
    {

        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(_maxConnections);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "wss"));
        _joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        NetworkManager.Singleton.StartHost();
        State = new CharacterSelectMenuState(this);
    }

    public void StartGameAsClient()
    {
        _joinCode = Canvas.transform.Find("MatchSearchMenu/Panel/CodeInput/TextArea/Text").gameObject.GetComponent<TextMeshProUGUI>().text.Replace("\0", "").Trim();
        _joinCode = _joinCode.Remove(_joinCode.Length - 1);
        StartClient(_joinCode);
    }

    //Método encargado de inicializar un runtime como cliente de una partida de juego que tiene de código "joinCode".
    async void StartClient(string joinCode)
    {
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "wss"));
            NetworkManager.Singleton.StartClient();

            State = new CharacterSelectMenuState(this);
        }
        catch (RelayServiceException e)
        {
            // Maneja la excepción aquí
            Debug.LogError($"Error al intentar unirse a la partida: {e.Message}");
            // Aquí podrías agregar lógica adicional para notificar al usuario, por ejemplo:
            Canvas.transform.Find("MatchSearchMenu/Panel/ErrorText").gameObject.SetActive(true);
        }
    }

    //void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    //OJO SI SE CAMBIA EL NOMBRE DE LA ESCENA
    //    if (SceneManager.GetActiveScene().name == "MPNetworkTest")
    //    {

    //    }
    //}
}
