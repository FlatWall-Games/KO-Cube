using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerDataManager : Singleton<PlayerDataManager>
{
    //Variable que indica el numero de personajes del juego
    const int numberOfCharacters = 6;

    //Atributos que se almacenaran
    string username;
    int coins;
    List<List<string>> characterSkins = new List<List<string>>(numberOfCharacters);
    //Por cada personaje se almacena una lista con el codigo de las skisn que posee, siendo 0 la skin por defecto.
    //Los codigos de los personajes (su orden en la lista) son:
    //VER EL ORDEN EN EL NETWORK MANAGER


    void Start()
    {
        //Inicizaliacion de la estructura
        for (int i = 0; i < numberOfCharacters; i++)
        {
            characterSkins.Add(new List<string>());
        }

        //Si existen datos guardados del jugador, los recuperamos y los almacenamos
        if (PlayerPrefs.HasKey("username"))
        {
            username = PlayerPrefs.GetString("username");
            coins = PlayerPrefs.GetInt("coins");

            for (int i = 0; i < numberOfCharacters; i++)
            {   //Recuperamos la cadena de skins de cada personaje
                string stringSkins = PlayerPrefs.GetString("Character" + i);
                //Convertimos en lista la cadena anterior
                List<string> skinList = stringSkins.Split('.').ToList();
                //Asignamos la lista recuperada
                characterSkins[i] = skinList;
            }

            Debug.Log("Datos recuperados con exito");
        }
        else
        {
            //Si no hay datos guardados, se crean con los valores iniciales
            Debug.Log("No hay datos guardados para este jugador. Creando estructura por defecto...");
            username = "jugador";
            coins = 10000;
            SetNameData(username); //Aqui habria que poner el nombre que escriba el jugador
            SetCoinsData(coins);
            SetSkinsData("0");
            SaveData();
        }
        MoneyText.Instance.UpdateMoney();

        Debug.Log("Ruta de PlayerPrefs en este sistema operativo: " + Application.persistentDataPath);
        //Descomentar esta linea para borrar los datos (esto solo esta aqui para pruebas)
        //DeleteData();
        PrintSkins();
    }

    //////////////////////// Metodos publicos para modificar datos desde otros scripts //////////////////////// 

    //Metodo para comprobar que se recupera bien la estructura
    public void PrintSkins()
    {
        foreach (List<string> skins in characterSkins) 
        { 
            foreach (string skin in skins)
            {
                Debug.Log(skin);
            }
        }
    }
    public void SetName(string name)
    {
        username = name;
        SetNameData(name);
        SaveData();
    }

    public void SetCoins(int amount)
    {
        coins = amount;
        SetCoinsData(amount);
        SaveData();
    }

    public void AddCoins(int amount)
    {
        SetCoins(coins + amount);
    }

    public void SetSkins(string skinCode, int charCode)
    {
        characterSkins[charCode].Add(skinCode);
        SetSkinsData(skinCode, charCode);
        SaveData();
    }

    public string GetName()
    {
        return username;
    }

    public int GetCoins()
    {
        return coins;
    }

    public List<string> GetSkins(int charCode)
    {
        return characterSkins[charCode];
    }

    public void SaveData()
    {
        PlayerPrefs.Save();
    }

    public void DeleteData()
    {
        PlayerPrefs.DeleteAll();
    }

    //////////////////////// Metodos privados que guardan los datos localmente con PlayerPrefs //////////////////////// 
    void SetNameData(string name)
    {
        PlayerPrefs.SetString("username", name);
    }

    void SetCoinsData(int amount)
    {
        PlayerPrefs.SetInt("coins", amount);
    }

    void SetSkinsData(string skinCode, int charCode = -1) 
    {
        //Quiere decir que hay que crear la estructura por defecto de las skins que tiene el jugador
        if (charCode == -1)
        {
            //Por cada personaje, se crea una lista con el codigo de las skins que tenga 
            for(int i = 0; i < numberOfCharacters; i++)
            {
                characterSkins[i].Add("0");
                PlayerPrefs.SetString("Character" + i, CreateStringSkins(characterSkins[i]));
            }
        }
        else
        {
            PlayerPrefs.SetString("Character" + charCode, CreateStringSkins(characterSkins[charCode]));
        }
    }

    string CreateStringSkins(List<string> skinList)
    {
        return string.Join(".", skinList);
    }
}
