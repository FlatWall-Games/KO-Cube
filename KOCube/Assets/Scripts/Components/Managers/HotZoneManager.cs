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
        gameModeUiText.text = $"¡Controla las zonas para ganar!";
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
        UpdateSliderClientRpc();
        base.PointScored(team);
    }

    [ClientRpc]
    private void UpdateSliderClientRpc()
    {
        if (PlayerBehaviour.ownerTag.Equals("Team1")) slider.value = pointsTeam1;
        else slider.value = pointsTeam2;
    }
}
