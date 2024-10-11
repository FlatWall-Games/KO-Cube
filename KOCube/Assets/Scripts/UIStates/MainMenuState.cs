using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuState : AUIState
{
    GameObject mainScreen;

    public MainMenuState(IUI context) : base(context)
    {
    }

    public override void Enter()
    {
        mainScreen = contextUI.Canvas.transform.Find("MainScreenMenu").gameObject;
        mainScreen.SetActive(true);
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
