using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HotZoneManager : AGameManager
{
    [SerializeField] private Slider slider;
    
    protected override void Awake()
    {
        base.Awake();
        pointsTeam1 = 50;
        pointsTeam2 = 50;
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void PointScored(string team) //Sumar un punto en hotzone conlleva quitarle un punto a tu rival
    {
        if (team.Equals("Team1")) pointsTeam2--;
        else pointsTeam1--;
        UpdateSliderClientRpc(pointsTeam1, pointsTeam2);
        base.PointScored(team);
    }

    [ClientRpc]
    private void UpdateSliderClientRpc(int points1, int points2)
    {
        if (PlayerBehaviour.ownerTag.Equals("Team1")) slider.value = points1;
        else slider.value = points2;
    }

    public override void InvertUI()
    {
        if (inverted) return;
        base.InvertUI();
    }

    public override void StartGame()
    {
        base.StartGame();
        SetTextClientRpc();
    }

    [ClientRpc]
    public void ZoneControlledClientRpc(string team)
    {
        if (team.Equals(PlayerBehaviour.ownerTag)) DisplayInformation("TU EQUIPO HA CONTROLADO UNA ZONA");
        else DisplayInformation("EL EQUIPO RIVAL HA CONTROLADO UNA ZONA");
    }

    [ClientRpc]
    private void SetTextClientRpc()
    {
        int numZonas = GameObject.FindObjectsOfType<HotZoneBehaviour>().Length;
        if (numZonas == 1) gameModeUiText.text = $"Controla 1 zona para ganar!";
        else gameModeUiText.text = $"Controla {numZonas} zonas para ganar!";
    }
}
