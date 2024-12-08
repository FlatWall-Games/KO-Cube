using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.VisualScripting;

public class AGameManager : NetworkBehaviour
{
    [SerializeField] protected int maxPoints;
    [SerializeField] protected TextMeshProUGUI pointsT1Text;
    [SerializeField] protected TextMeshProUGUI pointsT2Text;
    [SerializeField] protected GameObject LobbyUI;
    [SerializeField] protected TextMeshProUGUI timeLeftText;
    [SerializeField] protected float timeLeft = 120; //Duración de la partida en segundos
    [SerializeField] protected TextMeshProUGUI gameModeUiText; //Texto que aparece en la interfaz ingame sobrew este modo
    [SerializeField] private TextMeshProUGUI infoText;
    Coroutine infoCoroutine;

    protected int pointsTeam1 = 0;
    protected int pointsTeam2 = 0;
    protected bool gameStarted = false;
    protected bool acceptsClients = true;
    protected bool inverted = false;

    private bool _ready = false;
    [SerializeField] private GameObject readyButton;

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
                Invoke("DisableUIClientRpc", 1.5f);
            }
        }
    }

    public void EnableButton()
    {
        LobbyUI.SetActive(true);
    }

    public virtual void PointScored(string team)
    {
        if (team.Equals("Team1"))
        {
            if (++pointsTeam1 >= maxPoints)
            {
                StablishPlayersMovementClientRpc(false);
                EnterResultsAfterDelayClientRpc("Team1");
                Invoke("DisableUIClientRpc", 1.5f);
            }
        }
        else
        {
            if (++pointsTeam2 >= maxPoints)
            {
                StablishPlayersMovementClientRpc(false);
                EnterResultsAfterDelayClientRpc("Team2");
                Invoke("DisableUIClientRpc", 1.5f);
            }
        }
        UpdatePointsClientRpc(pointsTeam1, pointsTeam2);
    }

    [ClientRpc]
    protected void UpdatePointsClientRpc(int t1, int t2)
    {
        pointsTeam1 = t1;
        pointsTeam2 = t2;
        pointsT1Text.text = $"{t1}/{maxPoints}";
        pointsT2Text.text = $"{t2}/{maxPoints}";
    }

    public void ChangePlayerReadyStatus()
    {
        _ready = !_ready;
        readyButton.GetComponentInChildren<TextMeshProUGUI>().text = _ready ? "No Listo" : "Listo";
        readyButton.GetComponent<Button>().interactable = false;
        StartCoroutine(SetInteractableTrue());

        GameObject.FindObjectOfType<PlayersReadyManager>().PlayerReadyServerRpc(_ready);
    }

    IEnumerator SetInteractableTrue()
    {
        yield return new WaitForSeconds(0.5f);
        readyButton.GetComponent<Button>().interactable = true;
    }

    public virtual void StartGame()
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
            if (!movement) player.GetComponent<CharacterController>().enabled = false;
        }
        if(!movement) GameObject.FindObjectOfType<PlayersReadyManager>().GameEnded();
        StopAllCoroutines();
        infoText.text = "";
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

    public void SetAcceptClients(bool accept)
    {
        acceptsClients = accept;
    }

    public bool AcceptsClients()
    {
        return acceptsClients;
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

    [ClientRpc] 
    private void DisableUIClientRpc()
    {
        gameModeUiText.gameObject.SetActive(false);
        pointsT1Text.gameObject.SetActive(false);
        pointsT2Text.gameObject.SetActive(false);
        timeLeftText.gameObject.SetActive(false);
    }

    protected void DisplayInformation(string info, float speed = 1)
    {
        if (!gameStarted) return; //No hay información antes o después de la partida
        if(infoCoroutine == null) infoCoroutine = StartCoroutine(InfoWindow(info, speed));
    }

    private IEnumerator InfoWindow(string info, float speed)
    {
        infoText.text = info;
        infoText.GetComponent<Animator>().speed = speed;
        infoText.GetComponent<Animator>().Rebind();
        yield return new WaitForSeconds(3);
        infoText.text = "";
        infoCoroutine = null;
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
        AssignTimeAndPointsClientRpc(pointsTeam1, pointsTeam2, timeLeft, maxPoints,
           new ClientRpcParams
           {
               Send = new ClientRpcSendParams
               {
                   TargetClientIds = new ulong[] { serverParams.Receive.SenderClientId }
               }
           });
    }

    [ClientRpc]
    private void AssignTimeAndPointsClientRpc(int p1, int p2, float time,int maxPoints, ClientRpcParams clientParams = default)
    {
        gameStarted = true;
        pointsT1Text.text = $"{p1}/{maxPoints}";
        pointsT2Text.text = $"{p2}/{maxPoints}";
        timeLeft = time;
    }
}

