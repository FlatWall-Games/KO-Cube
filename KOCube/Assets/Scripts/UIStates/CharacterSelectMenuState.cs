using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterSelectMenuState : AUIState
{
    GameObject characterSelectUI;

    public CharacterSelectMenuState(IUI context) : base(context)
    {
    }
    public override void Enter()
    {
        characterSelectUI = contextUI.Canvas.transform.Find("CharacterSelectMenu/UI").gameObject;
        characterSelectUI.SetActive(true);
    }


    public override void Exit()
    {
        characterSelectUI.SetActive(false);
    }

    public override void FixedUpdate()
    {
    }

    public override void Update()
    {
    }
}
