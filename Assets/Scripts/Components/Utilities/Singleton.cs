using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
        //Al solo tener una escena, no nos interesa que se mantenga entre escenas. Esto permite que la escena de recargue
        //sin problemas 

        //DontDestroyOnLoad(gameObject);
    }
}
