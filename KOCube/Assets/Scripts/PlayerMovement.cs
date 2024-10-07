using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    CharacterController characterController;

    float yMovement = 0;
    float xMovement = 0;
    float zMovement = 0;
    Quaternion rotation;

    [SerializeField]
    float speed = 10f;
    float rotationSpeed = 10f;
    Vector3 predictedPosition;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        if (IsOwner)
        {
            GetComponent<PlayerInput>().enabled = true;
        }
    }

    private void Update()
    {
        if (IsOwner && !IsServer)
        {
            ClientMove();
        }

        if (IsServer)
        {
            ServerMove();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        xMovement = context.ReadValue<Vector2>()[0];
        zMovement = context.ReadValue<Vector2>()[1];

        if (!IsServer)
        {
            OnMoveServerRpc(context.ReadValue<Vector2>());
        }
    }

    [ServerRpc]
    public void OnMoveServerRpc(Vector2 context)
    {
        xMovement = context[0];
        zMovement = context[1];
    }

    public void ClientMove()
    {
        if (!characterController.isGrounded)
        {
            yMovement = -9.81f / speed;
        }

        Vector3 movement = new Vector3(xMovement, yMovement, zMovement);
        movement *= speed * Time.deltaTime;

        characterController.Move(movement);
        if (xMovement != 0 || zMovement != 0)
        {
            movement.y = 0f;
            rotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }

        //predictedPosition = transform.position;

        //SendPredictionServerRpc(predictedPosition);
    }

    public void ServerMove()
    {
        if (!characterController.isGrounded)
        {
            yMovement = -9.81f / speed;
        }
        Vector3 movement = new Vector3(xMovement, yMovement, zMovement);
        movement *= speed * Time.deltaTime;
        characterController.Move(movement);
        if (xMovement != 0 || zMovement != 0)
        {
            movement.y = 0f;
            rotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }

        if (!IsOwner)
        {
            SendPositionClientRpc(transform.position);
        }
        else 
        {
            SendHostPositionClientRpc(transform.position);
        }
        
    }
    /*
   [ServerRpc]
    public void SendPredictionServerRpc(Vector3 predictedPosition)
    {
        this.predictedPosition = predictedPosition;

        /*if (Vector3.Distance(transform.position, predictedPosition) > 0.05f)
        {
            Debug.Log("posicion interpolada");
            // Corrección de posición si hay desincronización
            transform.position = Vector3.Lerp(transform.position, predictedPosition, 0.5f);
        }

        SendPositionClientRpc(transform.position);
        ServerMove(;
    }*/


    [ClientRpc]
    public void SendPositionClientRpc(Vector3 newPosition)
    {
        if (Vector3.Distance(transform.position, newPosition) > 2f)
        {
            Debug.Log("posicion interpolada");
            // Corrección de posición si hay desincronizació
            transform.position = Vector3.Lerp(transform.position, newPosition, 0.5f);
        }
    }

    [ClientRpc]
    public void SendHostPositionClientRpc(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
}