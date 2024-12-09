using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : NetworkBehaviour
{
    //Variables de la interfaz
    public PreviewManager previewManager;
    public TextMeshProUGUI[] uiCharacterNames;
    public TextMeshProUGUI[] uiCharacterDescriptions;
    private int arrayIndex = 0;

    private TextMeshProUGUI uiCharacterName;
    private TextMeshProUGUI uiCharacterDescription;

    public bool isSpectator = false;
    public GameObject[] playerPrefabs; //Lista de prefabs de persooajes (Por si acaso, que su orden coincida con el de la lista del NetworkManager)

    public GameObject falseUI;
    
    private void Awake()
    {
        uiCharacterName = transform.Find("UI/CharacterSelection/CharacterNamePanel/CharacterName").GetComponent<TextMeshProUGUI>();
        uiCharacterDescription = transform.Find("UI/CharacterSelection/CharacterDescriptionPanel/CharacterDescription").GetComponent<TextMeshProUGUI>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    //Metodo que actualiza toda la informacion de la interfaz cuando se cambia de personaje
    public void ChangeCharacterUI(int value)
    {
        arrayIndex = previewManager.ChangePreview(value);
        //Actualizamos la imagen, el nombre y la descripcion
        ChangeCharacterName(arrayIndex);
        ChangeCharacterDescription(arrayIndex);
    }

    //Metodo que cambia el nombre del personaje
    private void ChangeCharacterName(int index)
    {
        uiCharacterName.text = uiCharacterNames[index].text;
    }

    //Metodo que cambia la descripcion del personaje
    private void ChangeCharacterDescription(int index)
    {
        uiCharacterDescription.text = uiCharacterDescriptions[index].text;
    }

    /////// Metodos Multijugador ///////
    
    //Este metodo habria que modificarlo para que el servidor compruebe si puede usar ese personaje y tal
    public void SelectCharacter()
    {
        falseUI.SetActive(true);

        if (IsServer)
        {
            isSpectator = false;
            // Si es el servidor, instanciamos el personaje directamente
            SpawnCharacterServer(arrayIndex);
        }
        else
        {
            // Si es un cliente, enviamos la solicitud al servidor
            RequestGameStartedServerRpc();
        }
        SelectionDone();
        UIManager.Instance.State = new GameState(UIManager.Instance);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestGameStartedServerRpc(ServerRpcParams serverRpcParams = default)
    {
        AssignGameStartedClientRpc(GameObject.FindObjectOfType<AGameManager>().AcceptsClients(),
        new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { serverRpcParams.Receive.SenderClientId }
            }
        });
    }

    [ClientRpc] 
    private void AssignGameStartedClientRpc(bool accepts, ClientRpcParams clientParams = default)
    {
        if (accepts)
        {
            SpawnCharacterRequestServerRpc(arrayIndex);
        }
        else
        {
            isSpectator = true;
            GameObject.FindWithTag("MainCamera").GetComponent<SpectatorCamera>().SetSpectate();
        }
    }

    // ServerRpc para que el servidor reciba la solicitud del cliente
    [ServerRpc(RequireOwnership = false)]
    public void SpawnCharacterRequestServerRpc(int characterIndex, ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;

        // Asegurarse de que el índice es válido
        if (characterIndex >= 0 && characterIndex < playerPrefabs.Length)
        {
            // Instanciar el personaje basado en la selección del cliente
            GameObject selectedPrefab = playerPrefabs[characterIndex];
            GameObject playerInstance = Instantiate(selectedPrefab);

            // Asociar el NetworkObject con el cliente
            playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        }
        else
        {
            Debug.LogWarning("Índice de personaje inválido ");
        }
    }

    private void SpawnCharacterServer(int characterIndex)
    {
        // Asegurarse de que el índice es válido
        if (characterIndex >= 0 && characterIndex < playerPrefabs.Length)
        {
            // Instanciar el personaje basado en la selección del cliente
            GameObject selectedPrefab = playerPrefabs[characterIndex];
            GameObject playerInstance = Instantiate(selectedPrefab);

            // Asociar el NetworkObject con el cliente
            playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(OwnerClientId);
        }
        else
        {
            Debug.LogWarning("Índice de personaje inválido");
        }
    }
    
    private void SelectionDone()
    {
        if (IsServer)
        {
            GameObject.FindObjectOfType<ModeSelector>().UpdateMode();
            GameObject.FindObjectOfType<MapSelector>().UpdateMap();
        }
        else
        {
            GameObject.FindObjectOfType<ModeSelector>().RequestModeServerRpc();
            GameObject.FindObjectOfType<MapSelector>().RequestMapServerRpc();
        }
        GameObject.FindObjectOfType<UIManager>().gui_visible = true;
    }
}
