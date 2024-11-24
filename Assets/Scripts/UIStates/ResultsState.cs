using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultsState : AUIState
{
    GameObject resultsScreen;

    public ResultsState(IUI context) : base(context)
    {
    }

    public override void Enter()
    {
        resultsScreen = contextUI.Canvas.transform.Find("ResultsScreenMenu").gameObject;
        resultsScreen.SetActive(true);
        PlayerBehaviour[] playerList = GameObject.FindObjectsOfType<PlayerBehaviour>();

        //Iteramos hasta encontrar al owner y aplicamos los colores segun el tag
        foreach (var player in playerList)
        {
            if (player.IsOwner)
            {
                UIManager.Instance.Canvas.transform.Find("ResultsScreenMenu/PersonalStats/Kills/KillsText").GetComponent<TextMeshProUGUI>().text = player.GetComponent<MatchStatsManager>().GetKills().ToString();
                UIManager.Instance.Canvas.transform.Find("ResultsScreenMenu/PersonalStats/Deaths/DeathsText").GetComponent<TextMeshProUGUI>().text = player.GetComponent<MatchStatsManager>().GetDeaths().ToString();
                UIManager.Instance.Canvas.transform.Find("ResultsScreenMenu/MatchResultLabel/FinalResultText").GetComponent<TextMeshProUGUI>().text = player.GetComponent<MatchStatsManager>().GetResults();
                if (player.GetComponent<MatchStatsManager>().GetResults().Equals("Victoria")) GameObject.FindObjectOfType<MusicManager>().PlaySong(3, 10, false);
                else GameObject.FindObjectOfType<MusicManager>().PlaySong(4, 10, false);
            }
        }
        
    }

    public override void Exit()
    {
        resultsScreen.SetActive(false);
    }

    public override void FixedUpdate()
    {
    }

    public override void Update()
    {
    }
}
