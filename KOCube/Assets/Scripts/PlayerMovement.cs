using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    CharacterController characterController;

    float xMovement = 0; //Movimiento en el eje x
    float yMovement = 0; //Movimiento en el eje y
    float zMovement = 0; //Movimiento en el eje z
    Quaternion rotation; //Rotación que hay que aplicar para que el personaje mire a donde se está moviendo

    [SerializeField]
    float speed = 10f; //Velocidad de movimiento
    float rotationSpeed = 10f; //Velocidad de rotación

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        if (IsOwner) //Encendemos el PlayerInput sólo para el personaje del jugador
        {
            GetComponent<PlayerInput>().enabled = true;
        }
    }

    private void Update()
    {
        if(IsOwner || IsServer) MovePlayer(); //Cada cliente sólo calcula su posición, el server calcula la de todos
        //El servidor manda la nueva posición a todos los jugadores después de haberla calculado:
        if (IsServer) 
        {
            if (!IsOwner) SendPositionClientRpc(transform.position, transform.rotation); //Función con interpolación para los clientes
            else SendHostPositionClientRpc(transform.position, transform.rotation); //Función sin interpolación para el host
        }
    }

    public void OnMove(InputAction.CallbackContext context) //Llamado desde el Player Input
    {
        if (!characterController.isGrounded) return;
        xMovement = context.ReadValue<Vector2>()[0];
        zMovement = context.ReadValue<Vector2>()[1];

        if (!IsServer) //El personaje del host se actualiza a sí mismo, así que no hace falta que llame a la función
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

    public void MovePlayer()
    {
        if (!characterController.isGrounded) //Se aplica gravedad, pues CharacterController no tiene gravedad de forma nativa
        {
            yMovement = -9.81f / speed; //Se tiene en cuenta que posteriormente se multiplica por la velocidad
        }
        Vector3 movement = new Vector3(xMovement, yMovement, zMovement);
        movement *= speed * Time.deltaTime;
        characterController.Move(movement);
        //Si el jugador se está moviendo se actualiza la rotación del personaje para que mire hacia donde se mueve
        if (xMovement != 0 || zMovement != 0)
        {
            movement.y = 0f; //Se anula el eje y para que no rote verticalmente
            rotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }
    }

    [ClientRpc]
    public void SendPositionClientRpc(Vector3 newPosition, Quaternion newRotation) //Manda la nueva posición interpolando
    {
        if (Vector3.Distance(transform.position, newPosition) > 2f)
        {
            Debug.Log("posicion interpolada");
            // Corrección de posición si hay desincronización
            transform.position = Vector3.Lerp(transform.position, newPosition, 0.5f * Time.deltaTime);
            transform.rotation = newRotation;
        }
    }

    [ClientRpc]
    public void SendHostPositionClientRpc(Vector3 newPosition, Quaternion newRotation)
    {
        transform.position = newPosition;
        transform.rotation = newRotation;
    }
}