using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : NetworkBehaviour
{
    //Variables de la interfaz
    public Sprite[] uiCharacterImages;
    public TextMeshProUGUI[] uiCharacterNames;
    public TextMeshProUGUI[] uiCharacterDescriptions;
    private int arrayIndex = 0;

    private Image uiCharacterPortrait;
    private TextMeshProUGUI uiCharacterName;
    private TextMeshProUGUI uiCharacterDescription;

    public GameObject[] playerPrefabs; //Lista de prefabs de persooajes (Por si acaso, que su orden coincida con el de la lista del NetworkManager)

    private void Awake()
    {
        uiCharacterPortrait = transform.Find("UI/CharacterImage").GetComponent<Image>();
        uiCharacterName = transform.Find("UI/CharacterNamePanel/CharacterName").GetComponent<TextMeshProUGUI>();
        uiCharacterDescription = transform.Find("UI/CharacterDescriptionPanel/CharacterDescription").GetComponent<TextMeshProUGUI>();
    }
    //Metodo que actualiza toda la informacion de la interfaz cuando se cambia de personaje
    public void ChangeCharacterUI(int value)
    {
        arrayIndex += value;

        //Controlamos que el indice que accede a los arrays no se salga
        //Si llega a valer la misma longitud que el array, igualamos el indice a cero
        //Si llega a valer menos de 0, lo igualamos al indice del ultimo elemento
        if( arrayIndex == uiCharacterImages.Length) arrayIndex = 0;
        else if( arrayIndex < 0) arrayIndex = uiCharacterImages.Length - 1;

        //Actualizamos la imagen, el nombre y la descripcion
        ChangeCharacterImage(arrayIndex);
        ChangeCharacterName(arrayIndex);
        ChangeCharacterDescription(arrayIndex);
    }

    //Metodo que cambia la imagen del personaje
    private void ChangeCharacterImage(int index)
    {
        uiCharacterPortrait.sprite = uiCharacterImages[index];
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
            // Si es el servidor, instanciamos el personaje directamente
            SpawnCharacterServer(arrayIndex);
        }
        else
        {
            // Si es un cliente, enviamos la solicitud al servidor
            SpawnCharacterRequestServerRpc(arrayIndex);
        }

        UIManager.Instance.State = new GameState(UIManager.Instance);
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
    private void Update()
    {

    }
}
