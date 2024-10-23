using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class DeathMatchManager : NetworkBehaviour
{
    [SerializeField] private int maxKills = 10;
    [SerializeField] private TextMeshProUGUI pointsT1Text;
    [SerializeField] private TextMeshProUGUI pointsT2Text;
    private int pointsTeam1 = 0;
    private int pointsTeam2 = 0;

    public void PlayerKilled(string tag)
    {
        if (tag.Equals("Team1"))
        {
            if(++pointsTeam2 >= maxKills) Debug.Log("Partida finalizada");
        }
        else
        {
            if(++pointsTeam1 >= maxKills) Debug.Log("Partida finalizada");
        }
        UpdatePointsClientRpc(pointsTeam1, pointsTeam2);
    }

    [ClientRpc] 
    private void UpdatePointsClientRpc(int t1, int t2)
    {
        pointsT1Text.text = t1.ToString();
        pointsT2Text.text = t2.ToString();
    }
}
