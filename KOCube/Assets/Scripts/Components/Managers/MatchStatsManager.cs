using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchStatsManager : MonoBehaviour
{
    [SerializeField] int kills;
    [SerializeField] int deaths;

    public int GetKills() { return kills; } 
    public int GetDeaths() { return deaths; } 

    public void AddKill() { kills++; }
    public void AddDeath() { deaths++; }
}
