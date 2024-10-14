using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyHostJoinState : AUIState
{
    GameObject mainScreen;
    GameObject matchSearchMenu;

    public LobbyHostJoinState(IUI context) : base(context)
    {
    }

    public override void Enter()
    {
        mainScreen = contextUI.Canvas.transform.Find("MainScreenMenu").gameObject;
        mainScreen.SetActive(true);
        matchSearchMenu = contextUI.Canvas.transform.Find("MatchSearchMenu").gameObject;
        matchSearchMenu.SetActive(true);
    }

    public override void Exit()
    {
        matchSearchMenu.SetActive(false);
        mainScreen.SetActive(false);
        contextUI.Canvas.transform.Find("Background").gameObject.SetActive(false);
    }

    public override void FixedUpdate()
    {
    }

    public override void Update()
    {
    }
}
