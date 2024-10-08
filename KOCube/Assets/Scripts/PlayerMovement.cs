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
        if (!IsServer) return;

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

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log("entrada recibida");
        OnMoveServerRpc(context.ReadValue<Vector2>());
    }

    [ServerRpc]
    public void OnMoveServerRpc(Vector2 context)
    {
        xMovement = context[0];
        zMovement = context[1];
    }
}