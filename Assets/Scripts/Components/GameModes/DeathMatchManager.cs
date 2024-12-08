using UnityEngine;

public class DeathMatchManager : AGameManager
{
    protected override void Awake()
    {
        base.Awake();
        gameModeUiText.text = $"Elimina a {maxPoints} enemigos para ganar!";
    }

    private void Start()
    {
        
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void InvertUI()
    {
        if (inverted) return;
        base.InvertUI();
    }

    public override void PointScored(string team)
    {
        if((pointsTeam1 == 5 && PlayerBehaviour.ownerTag.Equals("Team1")) || (pointsTeam2 == 5 && PlayerBehaviour.ownerTag.Equals("Team2")))
        {
            StartCoroutine(DisplayInformation("QUEDAN 5 BAJAS PARA GANAR"));
        }
        else if ((pointsTeam1 == 5 && PlayerBehaviour.ownerTag.Equals("Team2")) || (pointsTeam2 == 5 && PlayerBehaviour.ownerTag.Equals("Team1")))
        {
            StartCoroutine(DisplayInformation("QUEDAN 5 BAJAS PARA PERDER"));
        }
        base.PointScored(team);
    }
}