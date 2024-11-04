using UnityEngine;

public class DeathMatchManager : AGameManager
{
    protected override void Awake()
    {
        base.Awake();
        gameModeUiText.text = $"¡Elimina a {maxPoints} enemigos para ganar!";
    }

    private void Start()
    {
        
    }

    protected override void Update()
    {
        base.Update();
    }
}