using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserSetupState : AUIState
{
    GameObject userSetupScreen;

    public UserSetupState(IUI context) : base(context)
    {
    }

    public override void Enter()
    {
        userSetupScreen = contextUI.Canvas.transform.Find("SetupUserMenu").gameObject;
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
    }
}
