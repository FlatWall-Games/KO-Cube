using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitPosManager : MonoBehaviour
{
    [SerializeField] private Transform[] team1Transforms;
    [SerializeField] private Transform[] team2Transforms;
    //Contador de número de jugadores por equipo:
    private int numP1 = 0;
    private int numP2 = 0;

    public Transform RequestPos(string tag)
    {
        if (numP1 >= 3) numP1 = 0;
        if (numP2 >= 3) numP1 = 0;
        if (tag.Equals("Team1")) return team1Transforms[numP1++];
        else return team2Transforms[numP2++];
    }
}
