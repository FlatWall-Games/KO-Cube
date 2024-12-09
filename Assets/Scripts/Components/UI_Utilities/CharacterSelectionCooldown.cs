using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionCooldown : MonoBehaviour
{
    //Tiempo de espera en segundos para poder confirmar la seleccion al cambiar de personaje
    [SerializeField] float selectionCooldownTime;
    float timer = 0f;
    bool startTimer = true;

    //Referencia al boton al que queremos ponerle el cooldown
    Button button;

    private void Start()
    {
        button = GetComponent<Button>();
    }

    //Suma el tiempo pasado y cuando llega a selectionCooldownTime, hace que el boton se pueda pulsar
    void Update()
    {
        if (startTimer)
        {
            timer += Time.deltaTime;
        }

        if (timer >= selectionCooldownTime) 
        {
            button.interactable = true;
            startTimer = false;
            timer = 0f;
        }
    }

    //Invocado desde los botones que cambian a los personajes
    public void StartCooldown()
    {
        button.interactable = false;
        startTimer = true;
    }
}
