using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class AttackManager : NetworkBehaviour
{
    private bool shooting = false; //Indica si está disparando
    private Animator anim; //Animador del personaje
    private Vector3 controllerAimDirection = Vector3.zero; //Dirección de apuntado con el mando
    [SerializeField] private GameObject aimIndicator; //Indica la dirección hacia la que se apunta con el mando para que sea más fácil apuntar
    private bool hideAfterShooting = false; //Indica si el indicador se debe apagar tras disparar con el mando

    [Header("Basic attack:")]
    [SerializeField] private AmmoManager ammoManager; //Controlador de los básicos que posibilita o no el disparo
    [SerializeField] GameObject basicPrefab; //Prefab del básico
    [SerializeField] private Transform basicOrigin; //Posición desde la que salen los básicos

    [Header("Ult attack:")]
    [SerializeField] private UltManager ultManager; //Controlador de las ultis
    [SerializeField] GameObject ultPrefab; //Prefab del básico
    [SerializeField] private Transform ultOrigin; //Posición desde la que salen las ultis

    private void Awake()
    {
        anim = GetComponent<Animator>();
        aimIndicator.SetActive(false); //En principio sólo se activa cuando el jugador apunta
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
        if (!IsOwner) return; //No hacemos las operaciones para los demás jugadores al ser innecesarias e ineficientes
        //La posición del indicador es la del jugador (a nivel de suelo) más el vector de entrada del joystick derecho del mando
        aimIndicator.transform.position = new Vector3(transform.position.x, transform.position.y - 0.95f, transform.position.z) + controllerAimDirection * 1.5f;
        if (hideAfterShooting && !shooting)
        {
            hideAfterShooting = false;
            aimIndicator.SetActive(false);
        }
    }

    public void ShootBasic(InputAction.CallbackContext context) //Básico con el ratón
    {
        if (context.performed)
        {
            Shoot("BASIC");
        }
    }

    public void ShootUlt(InputAction.CallbackContext context) //Ulti con el ratón
    {
        if (context.performed)
        {
            Shoot("ULT");
        }
    }

    public void Shoot(string attackType)
    {
        //Se lanza un rayo desde la cámara que pasa por el ratón. Se rota al jugador para que mire hacia ese punto
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(hit.point.x - transform.position.x, 0, hit.point.z - transform.position.z));
            ShootServerRpc(lookRotation, attackType);
        }
    }

    public void Aim(InputAction.CallbackContext context) //Apuntado con el mando
    {
        Vector2 aimVector = context.ReadValue<Vector2>();
        if(!shooting) controllerAimDirection = new Vector3(aimVector.x, 0, aimVector.y); //Dirección a la que disparará el jugador
        if (aimVector != Vector2.zero) aimIndicator.SetActive(true); //Si está apuntado se activa el indicador
        else if (shooting) hideAfterShooting = true; //Si se deja de apuntar pero se está disparando se activa el booleano que lo desactiva tras disparar
        else aimIndicator.SetActive(false); //Si se deja de apuntar sin disparar desaparece el indicador
    }

    public void ShootBasicController(InputAction.CallbackContext context) //Básico con mando
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
        //Si no se está apuntando hay autoapuntado
        if (controllerAimDirection == Vector3.zero) lookRotation = Quaternion.LookRotation(transform.forward);
        else lookRotation = Quaternion.LookRotation(controllerAimDirection);
        ShootServerRpc(lookRotation, attackType);
    }

    [ServerRpc]
    private void ShootServerRpc(Quaternion lookRotation, string attackType) //El servidor gestiona si el disparo se puede hacer o no
    {
        if (shooting) return;
        
        if (attackType.Equals("BASIC"))
        {
            if (!ammoManager.ShootRequested()) return; //Si el AmmoManager no deja disparar la función acaba aquí
            shooting = true;
            transform.rotation = lookRotation; //Se rota al jugador hacia donde dispara
            anim.SetTrigger("ShootBasic"); //Compartido entre todos los clientes, lo que hace que el disparo aparezca para todo el mundo
        }
        else
        {
            if (!ultManager.RequestShoot()) return;
            shooting = true;
            transform.rotation = lookRotation;
            anim.SetTrigger("ShootUlt");
        }

        UpdateBarsClientRpc(attackType);
    }

    [ClientRpc]
    private void UpdateBarsClientRpc(string attackType)
    {
        Debug.Log(attackType);
        if (attackType.Equals("BASIC")) ammoManager.UpdateAmmoBar();
        else ultManager.UpdateBar();
    }

    public void ShootSingleBasicProjectile() //Llamado desde la animación de disparo
    {
        IAttack projectile = GameObject.Instantiate(basicPrefab, basicOrigin).GetComponent<IAttack>();
        projectile.SetTag(this.tag); //Le pone tag para que gestione colisiones, daño y curas
        projectile.SetAttacker(GetComponent<PlayerMovement>()); //Se configura para que sepa quién lanzó el ataque
    }

    public void ShootSingleUltProjectile() //Llamado desde la animación de disparo
    {
        IAttack projectile = GameObject.Instantiate(ultPrefab, ultOrigin).GetComponent<IAttack>();
        projectile.SetTag(this.tag); //Le pone tag para que gestione colisiones, daño y curas
        projectile.SetAttacker(GetComponent<PlayerMovement>()); //Se configura para que sepa quién lanzó el ataque
    }

    public bool IsShooting() //Llamado desde PlayerMovement para saber si rotar el jugador hacia donde mira o no
    {
        return shooting;
    }

    public void OnShootEnded() //Llamado desde la animación
    {
        shooting = false;
    }
}
