using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInputHandler : MonoBehaviour
{
    public TMP_InputField inputField; // Arrastra tu campo de texto en el inspector

    // Método que se llamará cuando el objeto sea clickeado
    public void OnSelect(string s)
    {
        Debug.Log("me has pulsado");
        // Activar el teclado en pantalla
        inputField.ActivateInputField();
        TouchScreenKeyboard.Open(inputField.textComponent.text, TouchScreenKeyboardType.Default); // Abre el teclado virtual
        Debug.Log("supuestamente abri el teclado");
        
    }
}