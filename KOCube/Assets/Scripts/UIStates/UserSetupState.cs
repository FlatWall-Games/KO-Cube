using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserSetupState : AUIState
{
    GameObject userSetupScreen; //Referencia al objeto que contiene la interfaz
    TextMeshProUGUI informationBoxText; //Referencia al texto que se muestra en la caja de avisos
    TMP_InputField nameInputField;  //Referencia al inputfield del nombre del jugador
    Button continueButton; //Referencia al boton de continuar

    string searchingText = "Buscando datos del Usuario";
    string succesSearchText = "¡Datos encontrados!";
    string failureSearchText = "No se han encontrado datos, introduce tu nombre";

    float maxSearchingTimer = Random.Range(0.5f, 3f);
    float searchingTimer = 0f;
    float textTimer = 0f;

    bool canContinue = false;

    public UserSetupState(IUI context) : base(context)
    {
    }

    public override void Enter()
    {
        //Inicializamos las variables
        userSetupScreen = contextUI.Canvas.transform.Find("SetUpUserMenu").gameObject;
        informationBoxText = userSetupScreen.transform.Find("InformationBox/Text").gameObject.GetComponent<TextMeshProUGUI>();
        nameInputField = userSetupScreen.transform.Find("NameInputField").gameObject.GetComponent<TMP_InputField>();
        continueButton = userSetupScreen.transform.Find("ContinueButton").gameObject.GetComponent<Button>();
        informationBoxText.text = searchingText;

        //Suscribimos el metodo Continue a la pulsacion del boton
        continueButton.onClick.AddListener(Continue);

        userSetupScreen.SetActive(true);
    }

    public override void Exit()
    {
        userSetupScreen.SetActive(false);
    }

    public override void FixedUpdate()
    {
    }

    public override void Update()
    {
        searchingTimer += Time.deltaTime;
        textTimer += Time.deltaTime;

        //Cada tercio del tiempo maximo se añadira un punto al texto (se suma 1 más porque al principio no tiene puntos)
        if (textTimer >= maxSearchingTimer / 4)
        {
            informationBoxText.text += ".";
            textTimer = 0f;
        }

        //Cuando se termina de emular la busqueda de datos
        if(searchingTimer > maxSearchingTimer)
        {
            if (PlayerDataManager.Instance.IsPlayerKnown())
            {
                canContinue = true;
                informationBoxText.text = succesSearchText;
                continueButton.gameObject.SetActive(true);
            }
            else
            {
                informationBoxText.text = failureSearchText;
                nameInputField.gameObject.SetActive(true);
                continueButton.gameObject.SetActive(true);
            }
        }
    }

    public void Continue()
    {
        if (canContinue || nameInputField.text != "")
        {
            PlayerDataManager.Instance.CreateDataSystem(nameInputField.text);
            contextUI.State = new MainMenuState(contextUI);
        }
        
    }
}
