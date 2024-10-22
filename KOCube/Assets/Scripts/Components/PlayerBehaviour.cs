using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class PlayerBehaviour : NetworkBehaviour
{
    //Variables de red:
    public struct InputPayload : INetworkSerializable
    {
        public int tick;
        public Vector2 inputVector;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref tick);
            serializer.SerializeValue(ref inputVector);
        }
    }

    public struct StatePayload : INetworkSerializable
    {
        public int tick;
        public Vector3 position;
        public Quaternion rotation;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref tick);
            serializer.SerializeValue(ref position);
            serializer.SerializeValue(ref rotation);
        }
    }

    CharacterController characterController;
    AttackManager basicShoot;
    public Renderer rend;
    private static string ownerTag = "Untagged";

    Vector2 input;

    float xMovement = 0;
    float yMovement = 0;
    float zMovement = 0;
    Quaternion rotation;

    [SerializeField]
    float speed = 5f;
    float rotationSpeed = 5f;
    bool canMove = true;

    //Netcode General:
    NetworkTimer timer;
    const float k_serverTickRate = 60f; //60 FPS
    const int k_bufferSize = 1024;

    //Netcode Cliente:
    CircularBuffer<StatePayload> clientStateBuffer;
    CircularBuffer<InputPayload> clientInputBuffer;

    StatePayload lastServerState;
    StatePayload lastProcessedState;

    //Netcode Servidor:
    CircularBuffer<StatePayload> serverStateBuffer;
    Queue<InputPayload> serverInputQueue;
    float reconciliationThreshold = 10f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        basicShoot = GetComponent<AttackManager>();
        rend = GetComponent<Renderer>();
        rend.material = new Material(rend.material); //Desvinculamos el material del objeto del original para que los cambios no afecten al resto

        timer = new NetworkTimer(k_serverTickRate);
        clientStateBuffer = new CircularBuffer<StatePayload>(k_bufferSize);
        clientInputBuffer = new CircularBuffer<InputPayload>(k_bufferSize);

        serverStateBuffer = new CircularBuffer<StatePayload> (k_bufferSize);
        serverInputQueue = new Queue<InputPayload> ();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer) this.tag = "Team" + (GameObject.FindObjectsOfType<PlayerBehaviour>().Length % 2 + 1).ToString(); //Se le asigna un equipo al entrar a la partida
        RequestTagServerRpc();
        if (IsOwner)
        {
            GetComponent<PlayerInput>().enabled = true;
        }
    }

    private void Update()
    {
        timer.Update(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        //if (!IsOwner) return;
        while (timer.ShouldTick())
        {
            HandleClientTick(input);
            HandleServerTick();
        }
    }

    void HandleServerTick()
    {
        if (!IsServer) return;

        int bufferIndex = -1;
        InputPayload inputPayload = default;
        while (serverInputQueue.Count > 0)
        {
            inputPayload = serverInputQueue.Dequeue();

            bufferIndex = inputPayload.tick % k_bufferSize;

            if (IsHost)
            {
                StatePayload statePayload1 = new StatePayload()
                {
                    tick = inputPayload.tick,
                    position = transform.position,
                    rotation = transform.rotation
                };
                serverStateBuffer.Add(statePayload1, bufferIndex);
                SendToClientRpc(statePayload1);
                continue;
            }

            StatePayload statePayload2 = ProcessMovement(inputPayload);
            serverStateBuffer.Add(statePayload2, bufferIndex);
        }

        if (bufferIndex == -1) return;

        SendToClientRpc(serverStateBuffer.Get(bufferIndex));
    }

    StatePayload SimulateMovement(InputPayload inputPayload)
    {
        Physics.simulationMode = SimulationMode.Script;

        Move(inputPayload.inputVector);
        Physics.Simulate(Time.fixedDeltaTime);
        Physics.simulationMode = SimulationMode.FixedUpdate;

        return new StatePayload()
        {
            tick = inputPayload.tick,
            position = transform.position,
            rotation = transform.rotation,
        };
    }

    [ClientRpc]
    void SendToClientRpc(StatePayload statePayload)
    {
        if (!IsOwner) return;
        lastServerState = statePayload;
    }

    void HandleClientTick(Vector2 input)
    {
        if (!IsClient || !IsOwner) return;

        int currentTick = timer.CurrentTick;
        int bufferIndex = currentTick % k_bufferSize;

        InputPayload inputPayload = new InputPayload()
        {
            tick = currentTick,
            inputVector = input
        };

        clientInputBuffer.Add(inputPayload, bufferIndex);
        SendToServerRpc(inputPayload);

        StatePayload statePayload = ProcessMovement(inputPayload);
        clientStateBuffer.Add(statePayload, bufferIndex);

        HandleServerReconciliation();
    }

    bool ShouldReconcile()
    {
        bool isNewServerState = !lastServerState.Equals(default);
        bool isLastStateUndefinedOrDifferent = lastProcessedState.Equals(default) || !lastProcessedState.Equals(lastServerState);

        return isNewServerState && isLastStateUndefinedOrDifferent;
    }

    void HandleServerReconciliation()
    {
        if (!ShouldReconcile()) return;

        float positionError;
        int bufferIndex;

        bufferIndex = lastServerState.tick % k_bufferSize;
        if (bufferIndex - 1 < 0) return; //No hay suficiente información para reconciliar

        StatePayload rewindState = IsHost ? serverStateBuffer.Get(bufferIndex - 1) : lastServerState; //Host RPCs se ejecutan inmediatamente, por lo que podemos usar el último estado del servidor.
        StatePayload clientState = IsHost ? clientStateBuffer.Get(bufferIndex - 1) : clientStateBuffer.Get(bufferIndex);
        positionError = Vector3.Distance(rewindState.position, clientState.position);

        if (positionError > reconciliationThreshold)
        {
            ReconcileState(rewindState);
        }

        lastProcessedState = rewindState;
    }

    void ReconcileState(StatePayload rewindState)
    {
        transform.position = rewindState.position;
        transform.rotation = rewindState.rotation;

        if (!rewindState.Equals(lastServerState)) return;

        clientStateBuffer.Add(rewindState, rewindState.tick % k_bufferSize);

        //Reproducimos todos los inputs desde el rewindState hasta el estado actual.
        int tickToReplay = lastServerState.tick;

        while (tickToReplay < timer.CurrentTick)
        {
            int bufferIndex = tickToReplay % k_bufferSize;
            StatePayload statePayload = ProcessMovement(clientInputBuffer.Get(bufferIndex));
            clientStateBuffer.Add(statePayload, bufferIndex);
            tickToReplay++;
        }
    }

    [ServerRpc]
    void SendToServerRpc(InputPayload input)
    {
        serverInputQueue.Enqueue(input);
    }

    StatePayload ProcessMovement(InputPayload input)
    {
        Move(input.inputVector);

        return new StatePayload()
        {
            tick = input.tick,
            position = transform.position,
            rotation = transform.rotation,
        };
    }

    void Move(Vector2 inputVector)
    {
        if (!characterController.isGrounded) //Simulamos gravedad ya que el CharacterController no la tiene de forma nativa
        {
            yMovement = -9.81f / speed; //Tenemos en cuenta que después se multiplica por la velocidad al vector entero
        }
        Vector3 movement = Vector3.zero;
        if (canMove)
        {
            movement = new Vector3(inputVector[0], yMovement, inputVector[1]);
        }
        else
        {
            movement = new Vector3(0f, yMovement, 0f);
        }
        movement *= speed * Time.deltaTime;
        characterController.Move(movement);
        if ((inputVector[0] != 0 || inputVector[1] != 0) && !basicShoot.IsShooting()) //Si se mueve y no está disparando mira hacia donde se mueve
        {
            movement.y = 0f; //Anulamos el eje y del movimiento para que rote en el eje deseado
            rotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
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
        PlayerBehaviour[] players = GameObject.FindObjectsOfType<PlayerBehaviour>();
        foreach(PlayerBehaviour player in players)
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