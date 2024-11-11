using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuState : AUIState
{
    GameObject mainScreen;
    TextMeshProUGUI nameText;

    public MainMenuState(IUI context) : base(context)
    {
    }

    public override void Enter()
    {
        mainScreen = contextUI.Canvas.transform.Find("MainScreenMenu").gameObject;
        mainScreen.SetActive(true);
        nameText = mainScreen.transform.Find("ProfileButton/Text").GetComponent<TextMeshProUGUI>();
        Debug.Log("el nombre es " + PlayerDataManager.Instance.GetName());
        nameText.text = PlayerDataManager.Instance.GetName();
        //EventSystem.current.SetSelectedGameObject(mainScreen.transform.Find("PlayButton").gameObject);
    }

    public override void Exit()
    {
        mainScreen.SetActive(false);
    }

    public override void FixedUpdate()
    {
    }

    public override void Update()
    {
    }
}
