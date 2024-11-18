using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HUD_ControllerIcons : MonoBehaviour
{
    //Arrays con los objetos que hay que esconder
    public GameObject[] mouseIcons;
    public GameObject[] controllerIcons;
    //Variable auxiliar para saber cuando ha habido un cambio en el mando (desconexiones o conexiones)
    bool controllerConected = false;

    public void Update() 
    {
        //Si se detecta que se ha conectado un mando
        if (Gamepad.current != null && !controllerConected)
        {
            MouseSwitch(false);
            ControllerSwitch(true);
            controllerConected = true;
        }
        //Si se detecta que se ha desconectado un mando
        else if (Gamepad.current == null && controllerConected)
        {
            MouseSwitch(true);
            ControllerSwitch(false);
            controllerConected = false;
        }
    }

    void MouseSwitch(bool state)
    {
        mouseIcons[0].SetActive(state);
        mouseIcons[1].SetActive(state);
    }

    void ControllerSwitch(bool state)
    {
        controllerIcons[0].SetActive(state);
        controllerIcons[1].SetActive(state);
    }
    
}
