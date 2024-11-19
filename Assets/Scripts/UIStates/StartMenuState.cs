using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartMenuState : AUIState
{
    GameObject startMenu;

    public StartMenuState(IUI context) : base(context)
    {
    }

    public override void Enter()
    {
        startMenu = contextUI.Canvas.transform.Find("StartMenu").gameObject;
        startMenu.SetActive(true);
        contextUI.Canvas.transform.Find("Shop").gameObject.SetActive(false);
    }

    public override void Exit()
    {
        startMenu.SetActive(false);
    }

    public override void FixedUpdate()
    {
    }

    public override void Update()
    {
    }
}
