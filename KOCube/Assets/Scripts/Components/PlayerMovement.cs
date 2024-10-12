using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using Cinemachine;

public class PlayerMovement : NetworkBehaviour
{
    CharacterController characterController;
    BasicShoot basicShoot;

    float xMovement = 0;
    float yMovement = 0;
    float zMovement = 0;
    Quaternion rotation;

    [SerializeField]
    float speed = 10f;
    float rotationSpeed = 10f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        basicShoot = GetComponent<BasicShoot>();
    }

    void Start()
    {
        this.tag = "Team" + (GameObject.FindObjectsOfType<PlayerMovement>().Length%2+1).ToString(); //Se le asigna un equipo al entrar a la partida
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
        Vector3 movement = new Vector3(xMovement, yMovement, zMovement);
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
}