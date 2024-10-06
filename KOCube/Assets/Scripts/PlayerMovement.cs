using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
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
    private void Update()
    {
        if (!characterController.isGrounded)
        {
            yMovement = -9.81f / speed;
        }
        Vector3 movement = new Vector3 (xMovement, yMovement, zMovement);
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
        xMovement = context.ReadValue<Vector2>()[0];
        zMovement = context.ReadValue<Vector2>()[1];
    }
}
