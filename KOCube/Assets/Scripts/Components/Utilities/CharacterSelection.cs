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
    public GameObject[] characterPreviews;
    public TextMeshProUGUI[] uiCharacterNames;
    public TextMeshProUGUI[] uiCharacterDescriptions;
    private int arrayIndex = 0;

    private TextMeshProUGUI uiCharacterName;
    private TextMeshProUGUI uiCharacterDescription;

    public bool isSpectator = true;
    public GameObject[] playerPrefabs; //Lista de prefabs de persooajes (Por si acaso, que su orden coincida con el de la lista del NetworkManager)
    
    private void Awake()
    {
        isSpectator = true;
        uiCharacterName = transform.Find("UI/CharacterSelection/CharacterNamePanel/CharacterName").GetComponent<TextMeshProUGUI>();
        uiCharacterDescription = transform.Find("UI/CharacterSelection/CharacterDescriptionPanel/CharacterDescription").GetComponent<TextMeshProUGUI>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        GameObject.FindObjectOfType<PlayersReadyManager>().AddPlayerServerRpc();
        foreach(GameObject preview in characterPreviews) { preview.SetActive(false); }
        ChangeCharacterUI(0);
    }

    //Metodo que actualiza toda la informacion de la interfaz cuando se cambia de personaje
    public void ChangeCharacterUI(int value)
    {
        characterPreviews[arrayIndex].SetActive(false);
        arrayIndex += value;
        //Controlamos que el indice que accede a los arrays no se salga
        //Si llega a valer la misma longitud que el array, igualamos el indice a cero
        //Si llega a valer menos de 0, lo igualamos al indice del ultimo elemento
        if( arrayIndex == characterPreviews.Length) arrayIndex = 0;
        else if( arrayIndex < 0) arrayIndex = characterPreviews.Length - 1;
        characterPreviews[arrayIndex].SetActive(true);
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
            isSpectator = false;
            SpawnCharacterRequestServerRpc(arrayIndex);
        }
        else
        {
            GameObject.FindWithTag("MainCamera").GetComponent<SpectatorCamera>().SetSpectate();
        }
    }

    // ServerRpc para que el servidor reciba la solicitud del cliente
    [ServerRpc(RequireOwnership = false)]
    public void SpawnCharacterRequestServerRpc(int characterIndex, ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;

        // Asegurarse de que el �ndice es v�lido
        if (characterIndex >= 0 && characterIndex < playerPrefabs.Length)
        {
            // Instanciar el personaje basado en la selecci�n del cliente
            GameObject selectedPrefab = playerPrefabs[characterIndex];
            GameObject playerInstance = Instantiate(selectedPrefab);

            // Asociar el NetworkObject con el cliente
            playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        }
        else
        {
            Debug.LogWarning("�ndice de personaje inv�lido ");
        }
    }

    private void SpawnCharacterServer(int characterIndex)
    {
        // Asegurarse de que el �ndice es v�lido
        if (characterIndex >= 0 && characterIndex < playerPrefabs.Length)
        {
            // Instanciar el personaje basado en la selecci�n del cliente
            GameObject selectedPrefab = playerPrefabs[characterIndex];
            GameObject playerInstance = Instantiate(selectedPrefab);

            // Asociar el NetworkObject con el cliente
            playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(OwnerClientId);
        }
        else
        {
            Debug.LogWarning("�ndice de personaje inv�lido");
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
