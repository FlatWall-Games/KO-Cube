using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsMenuState : AUIState
{
    GameObject creditsUI;

    public CreditsMenuState(IUI context) : base(context) 
    {
        creditsUI = contextUI.Canvas.transform.Find("CreditsMenu").gameObject;
        creditsUI.SetActive(true);
    }

    public override void Enter()
    {
        creditsUI.SetActive(true);
    }

    public override void Exit()
    {
        creditsUI.SetActive(false);
    }

    public override void FixedUpdate()
    {
    }

    public override void Update()
    {
    }
}
