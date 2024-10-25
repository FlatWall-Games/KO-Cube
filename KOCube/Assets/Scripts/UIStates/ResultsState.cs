using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultsState : AUIState
{
    GameObject resultsScreen;

    public ResultsState(IUI context) : base(context)
    {
    }

    public override void Enter()
    {
        resultsScreen = contextUI.Canvas.transform.Find("ResultsScreenMenu").gameObject;
        resultsScreen.SetActive(true);
    }

    public override void Exit()
    {
        resultsScreen.SetActive(false);
    }

    public override void FixedUpdate()
    {
    }

    public override void Update()
    {
    }
}
