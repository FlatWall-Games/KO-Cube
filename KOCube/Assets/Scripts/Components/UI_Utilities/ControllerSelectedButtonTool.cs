using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControllerSelectedButtonTool : MonoBehaviour
{
    // Añadir este script al boton que se quiere seleccionar automaticamente con el mando al activar la interfaz
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(this.gameObject);
    }
}
