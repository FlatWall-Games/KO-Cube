using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchStatsManager : MonoBehaviour
{
    [SerializeField] int kills;
    [SerializeField] int deaths;
    [SerializeField] string result;

    public int GetKills() { return kills; } 
    public int GetDeaths() { return deaths; } 

    public string GetResults() { return result; }
    public void SetResults(string result) { this.result = result; }

    public void AddKill() { kills++; }
    public void AddDeath() { deaths++; }
}
