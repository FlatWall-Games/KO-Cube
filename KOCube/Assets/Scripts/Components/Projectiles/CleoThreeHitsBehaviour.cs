using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleoThreeHitsBehaviour : MonoBehaviour
{
    [SerializeField]int succededHits = 0;
    
    public void AddHit()
    {
        succededHits++;
    }
    public void ResetHits() 
    {
        succededHits = 0;
    }
    public int GetHits()
    {
        return succededHits; 
    }
}
