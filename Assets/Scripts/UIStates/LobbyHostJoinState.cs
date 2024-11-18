using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
        matchSearchMenu = contextUI.Canvas.transform.Find("MatchSearchMenu").gameObject;
        matchSearchMenu.SetActive(true);
    }

    public override void Exit()
    {
        matchSearchMenu.SetActive(false);
    }

    public override void FixedUpdate()
    {
    }

    public override void Update()
    {
    }
}
