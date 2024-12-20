using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using System;

public class AttackManager : NetworkBehaviour
{
    private bool shooting = false; //Indica si est� disparando
    public bool carrying = false; //Indica si est� cargando con una bandera
    private Animator anim; //Animador del personaje
    private Vector3 controllerAimDirection = Vector3.zero; //Direcci�n de apuntado con el mando
    [SerializeField] private GameObject aimIndicator; //Indica la direcci�n hacia la que se apunta con el mando para que sea m�s f�cil apuntar
    private bool hideAfterShooting = false; //Indica si el indicador se debe apagar tras disparar con el mando

    [Header("Basic attack:")]
    [SerializeField] private BasicAmmoManager ammoManager; //Controlador de los b�sicos que posibilita o no el disparo
    [SerializeField] GameObject basicPrefab; //Prefab del b�sico
    [SerializeField] private Transform basicOrigin; //Posici�n desde la que salen los b�sicos

    [Header("Ult attack:")]
    [SerializeField] private UltManager ultManager; //Controlador de las ultis
    [SerializeField] GameObject ultPrefab; //Prefab del b�sico
    [SerializeField] private Transform ultOrigin; //Posici�n desde la que salen las ultis
    
    public EventHandler<string> Ulted; //Con el que Judy comunica que se ha teletransportado para el correcto funcionamiento de hotzone
    public EventHandler<string> OnAttack;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        aimIndicator.SetActive(false); //En principio s�lo se activa cuando el jugador apunta
    }

    private void Start()
    {
        if (!IsOwner)
        {
            ammoManager.HideBars();
            ultManager.HideBar();
        }
    }

    private void Update()
    {
        if (!IsOwner) return; //No hacemos las operaciones para los dem�s jugadores al ser innecesarias e ineficientes
        //La posici�n del indicador es la del jugador (a nivel de suelo) m�s el vector de entrada del joystick derecho del mando
        aimIndicator.transform.position = new Vector3(transform.position.x, transform.position.y - 0.95f, transform.position.z) + controllerAimDirection * 1.5f;
        if (hideAfterShooting && !shooting)
        {
            hideAfterShooting = false;
            aimIndicator.SetActive(false);
        }
    }

    public void ShootBasic(InputAction.CallbackContext context) //B�sico con el rat�n
    {
        if (context.performed)
        {
            Shoot("BASIC");
        }
    }

    public void ShootUlt(InputAction.CallbackContext context) //Ulti con el rat�n
    {
        if (context.performed)
        {
            Shoot("ULT");
        }
    }

    public void Shoot(string attackType)
    {
        //Se lanza un rayo desde la c�mara que pasa por el rat�n. Se rota al jugador para que mire hacia ese punto
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 3))
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(hit.point.x - transform.position.x, 0, hit.point.z - transform.position.z));
            ShootServerRpc(lookRotation, attackType);
        }
    }

    public void Aim(InputAction.CallbackContext context) //Apuntado con el mando
    {
        Vector2 aimVector = context.ReadValue<Vector2>();
        if (this.tag.Equals("Team1")) aimVector *= -1; //Igual que el movimiento, se invierte para que se apunte bien desde el otro equipo
        if(!shooting) controllerAimDirection = new Vector3(aimVector.x, 0, aimVector.y); //Direcci�n a la que disparar� el jugador
        if (aimVector != Vector2.zero) aimIndicator.SetActive(true); //Si est� apuntado se activa el indicador
        else if (shooting) hideAfterShooting = true; //Si se deja de apuntar pero se est� disparando se activa el booleano que lo desactiva tras disparar
        else aimIndicator.SetActive(false); //Si se deja de apuntar sin disparar desaparece el indicador
    }

    public void ShootBasicController(InputAction.CallbackContext context) //B�sico con mando
    {
        if (context.performed)
        {
            ShootController("BASIC");
        }
    }

    public void ShootUltController(InputAction.CallbackContext context) //Ulti con mando
    {
        if (context.performed)
        {
            ShootController("ULT");
        }
    }

    public void ShootController(string attackType)
    {
        Quaternion lookRotation;
        //Si no se est� apuntando hay autoapuntado
        if (controllerAimDirection == Vector3.zero) lookRotation = Quaternion.LookRotation(transform.forward);
        else lookRotation = Quaternion.LookRotation(controllerAimDirection);
        ShootServerRpc(lookRotation, attackType);
    }

    [ServerRpc]
    private void ShootServerRpc(Quaternion lookRotation, string attackType) //El servidor gestiona si el disparo se puede hacer o no
    {
        if (shooting || carrying) return;
        
        if (attackType.Equals("BASIC"))
        {
            if (!ammoManager.ShootRequested()) return; //Si el AmmoManager no deja disparar la funci�n acaba aqu�
            transform.rotation = lookRotation; //Se rota al jugador hacia donde dispara
            anim.SetTrigger("ShootBasic"); //Compartido entre todos los clientes, lo que hace que el disparo aparezca para todo el mundo
        }
        else
        {
            if (!ultManager.RequestShoot()) return;
            transform.rotation = lookRotation;
            anim.SetTrigger("ShootUlt");
            Ulted?.Invoke(this.gameObject, this.tag);
        }

        UpdateBarsClientRpc(attackType);
    }

    [ClientRpc]
    private void UpdateBarsClientRpc(string attackType)
    {
        shooting = true;
        OnAttack?.Invoke(this, attackType);
        if (attackType.Equals("BASIC")) ammoManager.UpdateAmmoBar();
        else ultManager.UpdateBar();
    }

    public void ShootSingleBasicProjectile() //Llamado desde la animaci�n de disparo
    {
        if (basicPrefab.GetComponent<IAttack>().IsNetworkObject() && !IsServer) return;
        GameObject projectileInstance = GameObject.Instantiate(basicPrefab, basicOrigin);
        IAttack projectile = projectileInstance.GetComponent<IAttack>();
        projectile.SetTag(this.tag); //Le pone tag para que gestione colisiones, da�o y curas
        projectile.SetAttacker(GetComponent<PlayerBehaviour>()); //Se configura para que sepa qui�n lanz� el ataque
        if (IsServer && basicPrefab.GetComponent<IAttack>().IsNetworkObject())
        {
            projectileInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(OwnerClientId);
        }
    }

    public void ShootSingleUltProjectile() //Llamado desde la animaci�n de disparo
    {
        if (ultPrefab.GetComponent<IAttack>().IsNetworkObject() && !IsServer) return;
        GameObject projectileInstance = GameObject.Instantiate(ultPrefab, ultOrigin);
        IAttack projectile = projectileInstance.GetComponent<IAttack>();
        projectile.SetTag(this.tag); //Le pone tag para que gestione colisiones, da�o y curas
        projectile.SetAttacker(GetComponent<PlayerBehaviour>()); //Se configura para que sepa qui�n lanz� el ataque
        if (IsServer && ultPrefab.GetComponent<IAttack>().IsNetworkObject())
        {
            projectileInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(OwnerClientId);
            UpdateShieldColorClientRpc(projectileInstance.GetComponent<NetworkObject>().NetworkObjectId, tag);
        }
    }

    public bool IsShooting() //Llamado desde PlayerMovement para saber si rotar el jugador hacia donde mira o no
    {
        return shooting;
    }

    public void OnShootEnded() //Llamado desde la animaci�n
    {
        shooting = false;
        if (IsServer) OnShootEndedClientRpc();
    }

    [ClientRpc]
    private void OnShootEndedClientRpc()
    {
        shooting = false;
    }

    [ClientRpc]
    private void UpdateShieldColorClientRpc(ulong projID, string tag)
    {
        var networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[projID];
        GameObject proj = networkObject.gameObject;
        proj.GetComponent<ShieldColor>().UpdateColor(tag);
    } 
}
