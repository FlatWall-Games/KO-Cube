using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControllerSelectedButtonTool : MonoBehaviour
{
    // Añadir este script al boton que se quiere seleccionar automaticamente con el mando al activar la interfaz
    public void OnEnable()
    {
        StartCoroutine(SetFocus());
    }

    IEnumerator SetFocus()
    {
        //Espera un momento antes de asignar el foco para asegurar que la interfaz este activada
        yield return new WaitForSeconds(0.1f);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
