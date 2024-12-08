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
        float currentTeamPoints;
        if (team.Equals(PlayerBehaviour.ownerTag)) //Si es tu equipo
        {
            if (team.Equals("Team1")) currentTeamPoints = pointsTeam1; //Se mira la puntuación perteneciente al equipo del jugador
            else currentTeamPoints = pointsTeam2;
            switch (currentTeamPoints)
            {
                case 1:
                    DisplayInformationClientRpc("PRIMERA CAPTURA DE TU EQUIPO");
                    break;
                case 2:
                    DisplayInformationClientRpc("UNA BANDERA MÁS PARA GANAR");
                    break;
            }
        }
        else
        {
            if (team.Equals("Team1")) currentTeamPoints = pointsTeam1; //Se mira la puntuación del equipo contrario
            else currentTeamPoints = pointsTeam2;
            switch (currentTeamPoints)
            {
                case 1:
                    DisplayInformationClientRpc("PRIMERA CAPTURA DEL EQUIPO RIVAL");
                    break;
                case 2:
                    DisplayInformationClientRpc("UNA BANDERA MÁS PARA PERDER");
                    break;
            }
        }
    }
}