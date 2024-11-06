using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class AGameManager : NetworkBehaviour
{
    [SerializeField] protected int maxPoints;
    [SerializeField] protected TextMeshProUGUI pointsT1Text;
    [SerializeField] protected TextMeshProUGUI pointsT2Text;
    [SerializeField] protected Button startGameButton;
    [SerializeField] protected TextMeshProUGUI timeLeftText;
    [SerializeField] protected float timeLeft = 120; //Duración de la partida en segundos
    [SerializeField] protected TextMeshProUGUI gameModeUiText; //Texto que aparece en la interfaz ingame sobrew este modo

    protected int pointsTeam1 = 0;
    protected int pointsTeam2 = 0;
    protected bool gameStarted = false;
    protected bool inverted = false;

    protected virtual void Awake()
    {
        Time.timeScale = 1;
    }

    protected virtual void Update()
    {
        if (gameStarted)
        {
            timeLeft -= Time.deltaTime;
            int roundedDuration = (int)Mathf.Ceil(timeLeft);
            timeLeftText.text = (roundedDuration / 60).ToString() + ":" + (roundedDuration % 60).ToString("D2");
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
                UIEnablerClientRpc(false);
            }
        }
    }

    public void EnableButton()
    {
        startGameButton.gameObject.SetActive(true);
    }

    public virtual void PointScored(string team)
    {
        if (team.Equals("Team1"))
        {
            if (++pointsTeam1 >= maxPoints)
            {
                StablishPlayersMovementClientRpc(false);
                EnterResultsScreenClientRpc("Team1");
                UIEnablerClientRpc(false);
            }
        }
        else
        {
            if (++pointsTeam2 >= maxPoints)
            {
                StablishPlayersMovementClientRpc(false);
                EnterResultsScreenClientRpc("Team2");
                UIEnablerClientRpc(false);
            }
        }
        UpdatePointsClientRpc(pointsTeam1, pointsTeam2);
    }

    [ClientRpc]
    protected void UpdatePointsClientRpc(int t1, int t2)
    {
        pointsT1Text.text = t1.ToString();
        pointsT2Text.text = t2.ToString();
    }

    public void PlayerReady()
    {
        GameObject.FindObjectOfType<PlayersReadyManager>().PlayerReadyServerRpc();
    }

    public void StartGame()
    {
        StablishPlayersMovementClientRpc(true);
        UIEnablerClientRpc(true);
    }

    [ClientRpc]
    protected void StablishPlayersMovementClientRpc(bool movement)
    {
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
            if (player.IsOwner)
            {
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

    public bool HasStarted()
    {
        return gameStarted;
    }

    public virtual void InvertUI()
    {
        GameObject.FindWithTag("Reversible").transform.localScale = new Vector3(-1, 1, 1);
        pointsT1Text.rectTransform.localPosition = new Vector3(-pointsT1Text.rectTransform.localPosition.x, pointsT1Text.rectTransform.localPosition.y, 0);
        pointsT2Text.rectTransform.localPosition = new Vector3(-pointsT2Text.rectTransform.localPosition.x, pointsT2Text.rectTransform.localPosition.y, 0);
        inverted = true;
    }

    [ClientRpc]
    private void UIEnablerClientRpc(bool state)
    {
        gameModeUiText.gameObject.SetActive(state);
        pointsT1Text.gameObject.SetActive(state);
        pointsT2Text.gameObject.SetActive(state);
        timeLeftText.gameObject.SetActive(state);
        GameObject.FindObjectOfType<UIManager>().gui_visible = false; //Deberá estar desactivado en todos los casos, al comenzar y acabar la partida
    }
}

