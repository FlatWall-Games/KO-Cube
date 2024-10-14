using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : AUIState
{
    GameObject gameUI;

    public GameState(IUI context) : base(context)
    {
    }

    public override void Enter()
    {
        gameUI = contextUI.Canvas.transform.Find("GameInterface").gameObject;
        gameUI.SetActive(true);
    }

    public override void Exit()
    {
        gameUI.SetActive(false);
    }

    public override void FixedUpdate()
    {
    }

    public override void Update()
    {
    }
}
