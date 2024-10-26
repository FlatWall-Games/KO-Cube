using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_CharactersIcon : MonoBehaviour
{
    //Portraits de los personajes del equipo 1
    [SerializeField] Image[] t1CharacterPortraits;
    int arrayIndexTeam1 = 0;

    //Portraits de los personajes del equipo 2
    [SerializeField] Image[] t2CharacterPortraits;
    int arrayIndexTeam2 = 0;

    //Array de imagenes de los personajes
    public Sprite[] uiCharacterImages;

    //Metodo que se llama para poner las imagenes de los personajes en la interfaz
    public void SetCharacterPortraits()
    {
        //Conseguimos una lista de todos los jugadores y segun su tag de equipo, se rellena la interfaz con las imagenes
        PlayerBehaviour[] playerList = GameObject.FindObjectsOfType<PlayerBehaviour>();

        foreach (var player in playerList) 
        {
            string playerTag = player.gameObject.tag;
            //CharacterID es el int que identifica de forma univoca al personaje
            int characterId = player.gameObject.GetComponent<CharacterInfo>().characterID;

            if (playerTag == "Team1") AddPortraitToTeam1(characterId);
            else AddPortraitToTeam2(characterId);
            Debug.Log("me ejecute");
        }
    }

    //Metodo que recibe el ID del personaje y pone su imagen correspondiente en la interfaz del equipo 1
    void AddPortraitToTeam1(int characterID)
    {
        t1CharacterPortraits[arrayIndexTeam1].sprite = uiCharacterImages[characterID];
        arrayIndexTeam1++;
    }

    //Metodo que recibe el ID del personaje y pone su imagen correspondiente en la interfaz del equipo 2
    void AddPortraitToTeam2(int characterID)
    {
        t2CharacterPortraits[arrayIndexTeam2].sprite = uiCharacterImages[characterID];
        arrayIndexTeam2++;
    }
}
