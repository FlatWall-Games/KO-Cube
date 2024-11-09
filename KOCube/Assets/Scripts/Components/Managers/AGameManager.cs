using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEditor.PackageManager.Requests;
using System.Collections;

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
                    EnterResultsAfterDelayClientRpc("Team1");
                }
                else if (pointsTeam2 > pointsTeam1)
                {
                    EnterResultsAfterDelayClientRpc("Team2");
                }
                else
                {
                    EnterResultsAfterDelayClientRpc("Empate");
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
                EnterResultsAfterDelayClientRpc("Team1");
                UIEnablerClientRpc(false);
            }
        }
        else
        {
            if (++pointsTeam2 >= maxPoints)
            {
                StablishPlayersMovementClientRpc(false);
                EnterResultsAfterDelayClientRpc("Team2");
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
        if (GameObject.FindObjectOfType<CharacterSelection>().isSpectator) return; //No queremos que se haga para los espectadores
        gameStarted = movement;
        PlayerBehaviour[] players = GameObject.FindObjectsOfType<PlayerBehaviour>();
        foreach (PlayerBehaviour player in players)
        {
            if (player.IsOwner) player.GetComponent<PlayerInput>().enabled = movement;
        }
        GameObject.FindObjectOfType<PlayersReadyManager>().GameEnded();
    }

    [ClientRpc]
    public void EnterResultsAfterDelayClientRpc(string tag)
    {
        StartCoroutine(EnterResults(tag));
    }

    IEnumerator EnterResults(string tag)
    {
        yield return new WaitForSeconds(1.5f);
        PlayerBehaviour[] players = GameObject.FindObjectsOfType<PlayerBehaviour>();
        bool hasOwner = false;
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
                hasOwner = true; //Los espectadores no tendrán owner
            }
        }
        if (!hasOwner) UIManager.Instance.EndGameSession();
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
        if (GameObject.FindObjectOfType<CharacterSelection>().isSpectator) return; //No queremos que se haga para los espectadores
        gameModeUiText.gameObject.SetActive(state);
        pointsT1Text.gameObject.SetActive(state);
        pointsT2Text.gameObject.SetActive(state);
        timeLeftText.gameObject.SetActive(state);
        GameObject.FindObjectOfType<UIManager>().gui_visible = false; //Deberá estar desactivado en todos los casos, al comenzar y acabar la partida
    }

    //////////////////////SÍNCRONIZACIÓN DE LOS ESPECTADORES:
    ///
    public void SyncSpectatorData()
    {
        gameModeUiText.gameObject.SetActive(true);
        pointsT1Text.gameObject.SetActive(true);
        pointsT2Text.gameObject.SetActive(true);
        timeLeftText.gameObject.SetActive(true);
        RequestTimeAndPointsServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestTimeAndPointsServerRpc(ServerRpcParams serverParams = default)
    {
        AssignTimeAndPointsClientRpc(pointsTeam1, pointsTeam2, timeLeft,
           new ClientRpcParams
           {
               Send = new ClientRpcSendParams
               {
                   TargetClientIds = new ulong[] { serverParams.Receive.SenderClientId }
               }
           });
    }

    [ClientRpc]
    private void AssignTimeAndPointsClientRpc(int p1, int p2, float time, ClientRpcParams clientParams = default)
    {
        gameStarted = true;
        pointsT1Text.text = p1.ToString();
        pointsT2Text.text = p2.ToString();
        timeLeft = time;
    }
}

