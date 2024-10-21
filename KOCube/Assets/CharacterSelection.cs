using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;

public class CharacterSelection : NetworkBehaviour
{
    public GameObject[] playerPrefabs; // Lista de prefabs de personajes (Por si acaso, que su orden coincida con el de la lista del NetworkManager)
    private bool characterSpawned = false; //Booleano para que solo pueda instanciar 1 personaje en tu runtime
    public void SelectCharacter(int characterIndex)
    {
        if (IsServer)
        {
            // Si es el servidor, instanciamos el personaje directamente
            SpawnCharacterServer(characterIndex);
        }
        else
        {
            // Si es un cliente, enviamos la solicitud al servidor
            SpawnCharacterRequestServerRpc(characterIndex);
        }

        characterSpawned = true;
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
            Debug.LogWarning("Índice de personaje inválido");
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
    //Hasta que no haya interfaz, el personaje se escoge con numeros

    private void Update()
    {
        if (characterSpawned) return;

        //Al pulsar la tecla, se llama a SelectCharacter pasandole su indice del array
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Debug.Log("Tecla 0 pulsada");
            SelectCharacter(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Tecla 0 pulsada");
            SelectCharacter(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Tecla 0 pulsada");
            SelectCharacter(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Tecla 0 pulsada");
            SelectCharacter(3);
        }
    }
}
