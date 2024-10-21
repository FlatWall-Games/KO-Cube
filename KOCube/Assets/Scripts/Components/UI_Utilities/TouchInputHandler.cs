using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInputHandler : MonoBehaviour
{
    public TMP_InputField inputField; // Arrastra tu campo de texto en el inspector
    private TouchScreenKeyboard keyboard;

    // Método que se llamará cuando el objeto sea clickeado
    public void OnSelect(string s)
    {
        Debug.Log("me has pulsado");
        // Activar el teclado en pantalla
        keyboard = TouchScreenKeyboard.Open("",TouchScreenKeyboardType.Default); // Abre el teclado virtual
        inputField.ActivateInputField();
        Debug.Log("supuestamente abri el teclado");
        
    }

    private void Update()
    {
        if (keyboard != null && keyboard.active)
        {
            inputField.text = keyboard.text;
        }
    }
}