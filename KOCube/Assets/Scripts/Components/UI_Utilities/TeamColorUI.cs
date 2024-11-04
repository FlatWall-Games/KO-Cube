using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TeamColorUI : MonoBehaviour
{
    //Caja de personajes de la interfaz de partida
    [SerializeField] Image charactersTeam1;
    [SerializeField] Image charactersTeam2;

    //Caja de puntuaciones de la interfaz de partida
    [SerializeField] Image statsTeam1;
    [SerializeField] Image statsTeam2;

    //Colroes que se usan para identificar a los dos equipos en la interfaz. Se ponen en el editor
    [SerializeField] Color yourTeamColor;
    [SerializeField] Color enemyTeamColor;

    //Se llama desde los scripts del modo de juego cuando la partida comienza
    public void SetColorUI()
    {
        //Conseguimos una lista de todos los jugadores
        PlayerBehaviour[] playerList = GameObject.FindObjectsOfType<PlayerBehaviour>();

        //Iteramos hasta encontrar al owner y aplicamos los colores segun el tag
        foreach (var player in playerList)
        {
            if(player.IsOwner) 
            {
                string playerTag = player.gameObject.tag; 

                charactersTeam1.color = (playerTag == "Team1") ? yourTeamColor : enemyTeamColor;
                charactersTeam2.color = (playerTag == "Team2") ? yourTeamColor : enemyTeamColor;
                statsTeam1.color = (playerTag == "Team1") ? yourTeamColor : enemyTeamColor;
                statsTeam2.color = (playerTag == "Team2") ? yourTeamColor : enemyTeamColor;
            }
        }
    }
}
