using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class BasicShoot : NetworkBehaviour
{
    private bool shooting = false;
    private bool canShoot = true;
    private Animator anim;
    [SerializeField] GameObject bulletPrefab;
    private Vector3 controllerAimDirection = Vector3.zero;
    [SerializeField] private GameObject aimIndicator;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        aimIndicator.SetActive(false);
    }

    private void Update()
    {
        aimIndicator.transform.position = new Vector3(transform.position.x, transform.position.y - 0.95f, transform.position.z) + controllerAimDirection * 1.5f;
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Se lanza un rayo desde la cámara que pasa por el ratón. Se rota al jugador para que mire hacia ese punto
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(hit.point.x - transform.position.x, 0, hit.point.z - transform.position.z));
                ShootServerRpc(lookRotation);
            }
        }
    }

    public void Aim(InputAction.CallbackContext context)
    {
        Vector2 aimVector = context.ReadValue<Vector2>();
        controllerAimDirection = new Vector3(aimVector.x, 0, aimVector.y);
        if (aimVector != Vector2.zero) aimIndicator.SetActive(true);
        else aimIndicator.SetActive(false);
    }

    public void ShootController(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Quaternion lookRotation;
            //Si no se está apuntando hay autoapuntado
            if (controllerAimDirection == Vector3.zero) lookRotation = Quaternion.LookRotation(transform.forward); 
            else lookRotation = Quaternion.LookRotation(controllerAimDirection);
            ShootServerRpc(lookRotation);
        }
    }

    [ServerRpc]
    private void ShootServerRpc(Quaternion lookRotation)
    {
        if (!canShoot) return;
        canShoot = false;
        shooting = true;
        transform.rotation = lookRotation;
        anim.SetTrigger("Shoot"); //Compartido entre todos los clientes, lo que hace que el disparo aparezca para todo el mundo
    }

    public void ShootSingleBullet() //Llamado desde la animación de disparo
    {
        IAttack projectile = GameObject.Instantiate(bulletPrefab, transform).GetComponent<IAttack>();
        projectile.SetTag(this.tag);
        projectile.SetAttacker(GetComponent<PlayerMovement>());
    }

    public bool IsShooting() //Llamado desde PlayerMovement para saber si rotar el jugador hacia donde mira o no
    {
        return shooting;
    }

    public void OnShootEnded() //Llamado desde la animación
    {
        canShoot = true;
        shooting = false;
    }
}
