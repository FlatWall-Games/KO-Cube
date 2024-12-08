using Unity.Netcode;
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
        base.PointScored(team);
        SetInfoClientRpc(team);
    }

    [ClientRpc]
    private void SetInfoClientRpc(string team)
    {
        float currentTeamPoints;
        if (team.Equals("Team1")) currentTeamPoints = pointsTeam1;
        else currentTeamPoints = pointsTeam2;

        if (team.Equals(PlayerBehaviour.ownerTag)) //Si es tu equipo
        {
            switch (currentTeamPoints)
            {
                case 1:
                    DisplayInformation("PRIMERA CAPTURA DE TU EQUIPO");
                    break;
                case 2:
                    DisplayInformation("UNA BANDERA MÁS PARA GANAR");
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (currentTeamPoints)
            {
                case 1:
                    DisplayInformation("PRIMERA CAPTURA DEL EQUIPO RIVAL");
                    break;
                case 2:
                    DisplayInformation("UNA BANDERA MÁS PARA PERDER");
                    break;
                default:
                    break;
            }
        }
    }
}