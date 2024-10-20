using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class PlayerMovement : NetworkBehaviour
{
    CharacterController characterController;
    AttackManager basicShoot;
    public Renderer rend;
    private static string ownerTag = "Untagged";

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
        basicShoot = GetComponent<AttackManager>();
        rend = GetComponent<Renderer>();
        rend.material = new Material(rend.material); //Desvinculamos el material del objeto del original para que los cambios no afecten al resto
    }

    void Start()
    {
        if (IsServer) this.tag = "Team" + (GameObject.FindObjectsOfType<PlayerMovement>().Length % 2 + 1).ToString(); //Se le asigna un equipo al entrar a la partida
        RequestTagServerRpc();
        if (IsOwner)
        {
            GetComponent<PlayerInput>().enabled = true;
        }
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
        characterController.Move(movement);
        if ((xMovement != 0 || zMovement != 0) && !basicShoot.IsShooting()) //Si se mueve y no está disparando mira hacia donde se mueve
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
            xMovement = context[0];
            zMovement = context[1];
    }

    [ServerRpc (RequireOwnership = false)]
    private void RequestTagServerRpc()
    {
        AssignTagClientRpc(this.tag);
    }

    [ClientRpc]
    private void AssignTagClientRpc(string tag)
    {
        this.tag = tag;
        if(IsOwner) ownerTag = tag;
        if(!ownerTag.Equals("Untagged")) InitializePlayersShaders();
    }

    private void InitializePlayersShaders()
    {
        PlayerMovement[] players = GameObject.FindObjectsOfType<PlayerMovement>();
        foreach(PlayerMovement player in players)
        {
            if (!player.tag.Equals(ownerTag))
            {
                player.rend.material.SetColor("_color", Color.red);
            }
        }
    }

    public void CanMove()
    {
        canMove = true;
    }
    public void NotMove()
    {
        canMove = false;
    }
}