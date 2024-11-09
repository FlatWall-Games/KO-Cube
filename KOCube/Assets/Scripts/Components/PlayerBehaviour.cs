using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerBehaviour : NetworkBehaviour
{
    CharacterController characterController;
    AttackManager attackManager;
    public Renderer rend;
    public static string ownerTag = "Untagged";

    float xMovement = 0;
    float yMovement = 0;
    float zMovement = 0;
    Quaternion rotation;

    [SerializeField]
    float speed = 10f;
    float rotationSpeed = 10f;
    bool canMove = true;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        attackManager = GetComponent<AttackManager>();
        //rend = GetComponent<Renderer>();
        //rend.material = new Material(rend.material); //Desvinculamos el material del objeto del original para que los cambios no afecten al resto
    }

    void Start()
    {
        if (IsServer)
        {
            this.tag = "Team" + (GameObject.FindObjectsOfType<PlayerBehaviour>().Length % 2 + 1).ToString(); //Se le asigna un equipo al entrar a la partida
            InitializePosition();
            if (this.tag.Equals("Team1"))
            {
                UpdateCameraOffsetClientRpc();
            }
            AssignTagClientRpc(this.tag);
        }
        else RequestTagServerRpc();

        if(IsOwner) GameObject.FindObjectOfType<AGameManager>().EnableButton();
    }

    private void Update()
    {
        if (!IsServer) return; //El servidor es el único que calcula las posiciones y rotación

        if (!characterController.isGrounded) //Simulamos gravedad ya que el CharacterController no la tiene de forma nativa
        {
            yMovement = -9.81f / speed; //Tenemos en cuenta que después se multiplica por la velocidad al vector entero
        }
        Vector3 movement = Vector3.zero;
        if (canMove)
        {
            movement = new Vector3(xMovement, yMovement, zMovement);
        }
        else
        {
            movement = new Vector3(0f, yMovement, 0f);
        }
        movement *= speed * Time.deltaTime;
        if (attackManager.carrying) movement *= 0.75f; //Los personajes que cargan con banderas son más lentos
        characterController.Move(movement);
        if ((xMovement != 0 || zMovement != 0) && !attackManager.IsShooting()) //Si se mueve y no está disparando mira hacia donde se mueve
        {
            movement.y = 0f; //Anulamos el eje y del movimiento para que rote en el eje deseado
            rotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }

    }

    public void OnMove(InputAction.CallbackContext context)
    {

        OnMoveServerRpc(context.ReadValue<Vector2>());

    }

    [ServerRpc]
    public void OnMoveServerRpc(Vector2 context)
    {
        if (this.tag.Equals("Team1")) context *= -1; //Se invierte el vector de entrada, pues la cámara está girada
        xMovement = context[0];
        zMovement = context[1];
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestTagServerRpc()
    {
        AssignTagClientRpc(this.tag);
    }

    [ClientRpc]
    private void AssignTagClientRpc(string tag)
    {
        this.tag = tag;
        if (IsOwner)
        {
            ownerTag = tag;
            if (ownerTag.Equals("Team2")) GameObject.FindObjectOfType<AGameManager>().InvertUI(); //Se hace que el equipo del jugador esté siempre a la izquierda
            GameObject.FindObjectOfType<TeamColorUI>().SetColorUI();
        }
        if (!ownerTag.Equals("Untagged")) //Sólo se hará lo siguiente cuando el personaje del jugador esté inicializado
        {
            InitializePlayersShaders();
            GameObject.FindObjectOfType<HUD_CharactersIcon>().SetCharacterPortraits();
        }
    }

    private void InitializePlayersShaders()
    {
        PlayerBehaviour[] players = GameObject.FindObjectsOfType<PlayerBehaviour>();
        foreach (PlayerBehaviour player in players)
        {
            if (!player.tag.Equals(ownerTag))
            {
                player.rend.material.SetColor("_color", Color.red);
            }
        }
    }

    [ServerRpc]
    public void TeleportPlayerServerRpc(Vector3 newPos) 
    {
        this.GetComponent<Collider>().enabled = false;
        this.GetComponent<CharacterController>().enabled = false;
        transform.position = newPos;
        this.GetComponent<CharacterController>().enabled = true;
        this.GetComponent<Collider>().enabled = true;
    }

    public void CanMove()
    {
        canMove = true;
    }
    public void NotMove()
    {
        canMove = false;
    }

    public void InitializePosition()
    {
        Transform initTransform = GameObject.FindObjectOfType<InitPosManager>().RequestPos(this.tag);
        GetComponent<CharacterController>().enabled = false;
        this.transform.position = initTransform.position;
        this.transform.rotation = initTransform.rotation;
        GetComponent<CharacterController>().enabled = true;
        ResetUltClientRpc();
    }

    [ClientRpc]
    private void UpdateCameraOffsetClientRpc()
    {
        if (!IsOwner) return;
        CinemachineTransposer t = GameObject.FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>();
        t.m_FollowOffset = new Vector3(t.m_FollowOffset.x, t.m_FollowOffset.y, -t.m_FollowOffset.z);
    }

    [ClientRpc]
    public void AddKillsClientRpc()
    {
        GetComponent<MatchStatsManager>().AddKill();
    }

    [ClientRpc]
	private void ResetUltClientRpc()
    {
        transform.Find("UltManager").GetComponent<UltManager>().UpdateBar();
    }
}