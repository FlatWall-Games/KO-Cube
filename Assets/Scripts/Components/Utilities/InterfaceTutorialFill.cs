using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceTutorialFill : MonoBehaviour
{
    //Refefencia a la imagen de fondo que se va rellenando
    Image fillingBackground;

    //Obtenemos el componente
    void Start()
    {
        fillingBackground = GetComponent<Image>();
    }

    
    void Update()
    {
        fillingBackground.fillAmount += 0.3f * Time.deltaTime;
        //Reiniciamos el fondo cuando llegue a 1
        fillingBackground.fillAmount = (fillingBackground.fillAmount == 1)? 0 : fillingBackground.fillAmount;
    }
}
