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

    public override void PointScored(string team)
    {
        if (team.Equals(PlayerBehaviour.ownerTag))
        {
            StartCoroutine(DisplayInformation("TU EQUIPO HA CAPTURADO UNA BANDERA"));
        }
        else
        {
            StartCoroutine(DisplayInformation("EL EQUIPO ENEMIGO HA CAPTURADO UNA BANDERA"));
        }
        base.PointScored(team);
    }
}