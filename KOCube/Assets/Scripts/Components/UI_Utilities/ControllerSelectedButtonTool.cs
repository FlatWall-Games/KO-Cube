using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControllerSelectedButtonTool : MonoBehaviour
{
    public GameObject focusedElement;
    // A�adir este script al boton que se quiere seleccionar automaticamente con el mando al activar la interfaz
    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(focusedElement);
    }
}
