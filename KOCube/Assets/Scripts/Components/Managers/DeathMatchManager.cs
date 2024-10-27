using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DeathMatchManager : NetworkBehaviour
{
    [SerializeField] private int maxKills;
    [SerializeField] private TextMeshProUGUI pointsT1Text;
    [SerializeField] private TextMeshProUGUI pointsT2Text;
    [SerializeField] private Button startGameButton;
    [SerializeField] private TextMeshProUGUI timeLeftText;
    [SerializeField] private float timeLeft = 120; //Duración de la partida en segundos
    [SerializeField] private TextMeshProUGUI gameModeUiText; //Texto que aparece en la interfaz ingame sobrew este modo

    private int pointsTeam1 = 0;
    private int pointsTeam2 = 0;
    private bool gameStarted = false;

    private void Awake()
    {
        Time.timeScale = 0;
        gameModeUiText.text = $"¡Elimina a {maxKills} enemigos para ganar!";
    }

    private void Update()
    {
        if (gameStarted)
        {
            timeLeft -= Time.deltaTime;
            int roundedDuration = (int)Mathf.Ceil(timeLeft);
            timeLeftText.text = (roundedDuration / 60).ToString() + ":" + (roundedDuration % 60).ToString();
            if (IsServer && timeLeft <= 0)
            {
                StablishPlayersMovementClientRpc(false);
                if (pointsTeam1 > pointsTeam2)
                {
                    EnterResultsScreenClientRpc("Team1");
                }
                else if (pointsTeam2 > pointsTeam1)
                {
                    EnterResultsScreenClientRpc("Team2");
                }
                else 
                {
                    EnterResultsScreenClientRpc("Empate");
                }
                
            }
        }
    }

    public void EnableButton()
    {
        startGameButton.gameObject.SetActive(true);
    }

    public void PlayerKilled(string tag)
    {
        if (tag.Equals("Team1"))
        {
            if (++pointsTeam2 >= maxKills) { 
                StablishPlayersMovementClientRpc(false);
                EnterResultsScreenClientRpc("Team2");
            }
        }
        else
        {
            if (++pointsTeam1 >= maxKills)
            {
                StablishPlayersMovementClientRpc(false);
                EnterResultsScreenClientRpc("Team1");
            }
        }
        UpdatePointsClientRpc(pointsTeam1, pointsTeam2);
    }

    [ClientRpc] 
    private void UpdatePointsClientRpc(int t1, int t2)
    {
        pointsT1Text.text = t1.ToString();
        pointsT2Text.text = t2.ToString();
    }

    public void StartGame()
    {
        GameObject.FindObjectOfType<UIManager>().enabled = false;
        StablishPlayersMovementClientRpc(true);
    }

    [ClientRpc]
    private void StablishPlayersMovementClientRpc(bool movement)
    {
        //Aplica los colores de la interfaz cuando comienza la partida 
        GameObject.FindObjectOfType<TeamColorUI>().SetColorUI();

        pointsT1Text.gameObject.SetActive(true);
        pointsT2Text.gameObject.SetActive(true);
        timeLeftText.gameObject.SetActive(true);
        if (movement) Time.timeScale = 1;
        else Time.timeScale = 0;
        gameStarted = movement;
        PlayerBehaviour[] players = GameObject.FindObjectsOfType<PlayerBehaviour>();
        foreach (PlayerBehaviour player in players)
        {
            if (player.IsOwner) player.GetComponent<PlayerInput>().enabled = movement;
        }
    }

    [ClientRpc]
    public void EnterResultsScreenClientRpc(string tag)
    {
        PlayerBehaviour[] players = GameObject.FindObjectsOfType<PlayerBehaviour>();
        foreach (PlayerBehaviour player in players)
        {
            if (player.IsOwner) {
                if (player.tag.Equals(tag))
                {
                    player.GetComponent<MatchStatsManager>().SetResults("Victoria");
                }
                else if (tag.Equals("Empate")) 
                {
                    player.GetComponent<MatchStatsManager>().SetResults("Empate");
                }
                else
                {
                    player.GetComponent<MatchStatsManager>().SetResults("Derrota");
                }
            }
        }
        UIManager.Instance.State = new ResultsState(UIManager.Instance);
    }
}
