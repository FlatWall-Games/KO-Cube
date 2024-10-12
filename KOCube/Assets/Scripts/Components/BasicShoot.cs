using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class BasicShoot : NetworkBehaviour
{
    private bool shooting = false; //Indica si est� disparando
    [SerializeField] private AmmoManager ammoManager; //Controlador de las balas que posibilita o no el disparo
    private Animator anim; //Animador del personaje
    [SerializeField] GameObject bulletPrefab; //Prefab de la bala que lanza este ataque
    private Vector3 controllerAimDirection = Vector3.zero; //Direcci�n de apuntado con el mando
    [SerializeField] private GameObject aimIndicator; //Indica la direcci�n hacia la que se apunta con el mando para que sea m�s f�cil apuntar

    private void Awake()
    {
        anim = GetComponent<Animator>();
        aimIndicator.SetActive(false); //En principio s�lo se activa cuando el jugador apunta
    }

    private void Update()
    {
        //La posici�n del indicador es la del jugador (a nivel de suelo) m�s el vector de entrada del joystick derecho del mando
        aimIndicator.transform.position = new Vector3(transform.position.x, transform.position.y - 0.95f, transform.position.z) + controllerAimDirection * 1.5f;
    }

    public void Shoot(InputAction.CallbackContext context) //Disparo con el rat�n
    {
        if (context.performed)
        {
            //Se lanza un rayo desde la c�mara que pasa por el rat�n. Se rota al jugador para que mire hacia ese punto
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(hit.point.x - transform.position.x, 0, hit.point.z - transform.position.z));
                ShootServerRpc(lookRotation);
            }
        }
    }

    public void Aim(InputAction.CallbackContext context) //Apuntado con el mando
    {
        Vector2 aimVector = context.ReadValue<Vector2>();
        controllerAimDirection = new Vector3(aimVector.x, 0, aimVector.y); //Direcci�n a la que disparar� el jugador
        if (aimVector != Vector2.zero) aimIndicator.SetActive(true); //Si est� apuntado se activa el indicador, si no se desactiva
        else aimIndicator.SetActive(false);
    }

    public void ShootController(InputAction.CallbackContext context) //Disparo con mando
    {
        if (context.performed)
        {
            Quaternion lookRotation;
            //Si no se est� apuntando hay autoapuntado
            if (controllerAimDirection == Vector3.zero) lookRotation = Quaternion.LookRotation(transform.forward); 
            else lookRotation = Quaternion.LookRotation(controllerAimDirection);
            ShootServerRpc(lookRotation);
        }
    }

    [ServerRpc]
    private void ShootServerRpc(Quaternion lookRotation) //El servidor gestiona si el disparo se puede hacer o no
    {
        if (!ammoManager.ShootRequested()) return; //Si el AmmoManager no deja disparar la funci�n acaba aqu�
        shooting = true;
        transform.rotation = lookRotation; //Se rota al jugador hacia donde dispara
        anim.SetTrigger("Shoot"); //Compartido entre todos los clientes, lo que hace que el disparo aparezca para todo el mundo
    }

    public void ShootSingleBullet() //Llamado desde la animaci�n de disparo
    {
        IAttack projectile = GameObject.Instantiate(bulletPrefab, transform).GetComponent<IAttack>();
        projectile.SetTag(this.tag); //Le pone tag para que gestione colisiones, da�o y curas
        projectile.SetAttacker(GetComponent<PlayerMovement>()); //Se configura para que sepa qui�n lanz� el ataque
    }

    public bool IsShooting() //Llamado desde PlayerMovement para saber si rotar el jugador hacia donde mira o no
    {
        return shooting;
    }

    public void OnShootEnded() //Llamado desde la animaci�n
    {
        shooting = false;
    }
}
