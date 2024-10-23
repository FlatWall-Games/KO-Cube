using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonColorManager : MonoBehaviour
{
    Button button;
    ColorBlock colorBlock;
    Color normalColor = new Color(1, 1, 1); //Color normal
    Color hoverColor = new Color(0.92f, 0.79f, 0.79f); //Color al estar encima (debe coincidir con el de seleccionar en nuestro caso)
    Color pressedColor = new Color(0.92f, 0.58f, 0.58f); //Color al presionar
    Color disabledColor = new Color(0.76f, 0.76f, 0.76f); //Color de estar desactivado (tick de interactable desactivado en el editor)
    Color selectedColor = new Color(0.92f, 0.79f, 0.79f); //Color de estar seleccionado con mando

    // Start is called before the first frame update
    void Awake()
    {
        button = GetComponent<Button>();
        colorBlock = button.colors;

        colorBlock.normalColor = normalColor;
        colorBlock.highlightedColor = hoverColor;
        colorBlock.pressedColor = pressedColor;
        colorBlock.disabledColor = disabledColor;
        colorBlock.selectedColor = selectedColor;

        button.colors = colorBlock;
    }
}
