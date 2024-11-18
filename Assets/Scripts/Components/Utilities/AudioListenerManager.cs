using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerManager : MonoBehaviour
{
    private Transform targetTransform = null; //Posici�n del personaje del jugador, al que el AudioListener seguir�
    
    void Update() 
    {
        //El AudioListener sigue la posici�n, no la rotaci�n, para no provocar efectos raros desde el punto de vista cenital
        if(targetTransform != null)
        {
            this.transform.position = targetTransform.position;
        }
    }

    public void SetTransform(Transform t)
    {
        targetTransform = t; 
    }
}
