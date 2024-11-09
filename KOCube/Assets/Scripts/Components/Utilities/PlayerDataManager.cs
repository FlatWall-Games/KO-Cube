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
    List<List<bool>> characterSkins = new List<List<bool>>(numberOfCharacters);
    int[] activeSkins = new int[numberOfCharacters];
    //Por cada personaje se almacena una lista con el codigo de las skisn que posee, siendo 0 la skin por defecto.
    //Los codigos de los personajes (su orden en la lista) son:
    //VER EL ORDEN EN EL NETWORK MANAGER


    void Start()
    {
        //Inicizaliacion de la estructura
        for (int i = 0; i < numberOfCharacters; i++)
        {
            characterSkins.Add(new List<bool>());
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
                List<bool> skinBools = new List<bool>();
                foreach(string skin in skinList)
                {
                    skinBools.Add(bool.Parse(skin));
                }
                //Asignamos la lista recuperada
                characterSkins[i] = skinBools;
                SkinManager.Instance.RestoreData(i, characterSkins[i]);

                //Recuperamos también la lista de skins activas
                activeSkins[i] = PlayerPrefs.GetInt("ActiveSkin" + i);
            }
            SkinManager.Instance.RestoreActiveSkins(activeSkins);

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
            SetSkinsData(-1);
            SaveData();
            for(int i = 0; i < numberOfCharacters; i++)
            {
                SkinManager.Instance.RestoreData(i, characterSkins[i]);
                activeSkins[i] = 0;
            }
            SkinManager.Instance.RestoreActiveSkins(activeSkins);
        }
        MoneyText.Instance.UpdateMoney();

        Debug.Log("Ruta de PlayerPrefs en este sistema operativo: " + Application.persistentDataPath);
        //Descomentar esta linea para borrar los datos (esto solo esta aqui para pruebas)
        //DeleteData();
    }

    //////////////////////// Metodos publicos para modificar datos desde otros scripts //////////////////////// 

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

    public bool AddCoins(int amount)
    {
        if (coins + amount < 0) return false;
        SetCoins(coins + amount);
        return true;
    }

    public void SetSkins(int charCode, List<bool> list)
    {
        characterSkins[charCode] = list;
        SetSkinsData(charCode);
        SaveData();
    }

    public void SetActiveSkins(int[] activeSkins)
    {
        this.activeSkins = activeSkins;
        SetActiveSkinsData();
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

    public List<bool> GetSkins(int charCode)
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

    void SetSkinsData(int charCode = -1) 
    {
        //Quiere decir que hay que crear la estructura por defecto de las skins que tiene el jugador
        if (charCode == -1)
        {
            List<bool> newList = new List<bool>() { true, false, false, false };
            for(int i = 0; i < numberOfCharacters; i++)
            {
                characterSkins[i].Clear();
                characterSkins[i] = newList;
                PlayerPrefs.SetString("Character" + i, CreateStringSkins(characterSkins[i]));
            }
        }
        else
        {
            PlayerPrefs.SetString("Character" + charCode, CreateStringSkins(characterSkins[charCode]));
        }
    }

    void SetActiveSkinsData()
    {
        for(int i = 0; i < activeSkins.Length; i++)
        {
            PlayerPrefs.SetInt("ActiveSkin" + i, activeSkins[i]);
        }
    }

    string CreateStringSkins(List<bool> skinList)
    {
        return string.Join(".", skinList);
    }
}
