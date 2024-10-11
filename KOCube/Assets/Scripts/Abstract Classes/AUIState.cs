using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AUIState : IState
{
    protected IUI contextUI;

    public AUIState(IUI context)
    {
        contextUI = context;
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void FixedUpdate();
    public abstract void Update();
}
