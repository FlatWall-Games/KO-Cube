using UnityEngine;

public class CaptureFlagManager : AGameManager
{
    protected override void Awake()
    {
        base.Awake();
        gameModeUiText.text = $"Captura {maxPoints} banderas para ganar!";
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void InvertUI()
    {
        if (inverted) return;
        FlagBehaviour[] flags = GameObject.FindObjectsOfType<FlagBehaviour>();
        foreach (FlagBehaviour f in flags) f.ToggleMaterial();
        base.InvertUI();
    }
}