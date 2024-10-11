using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class BasicShoot : NetworkBehaviour
{
    private bool shooting = false;
    private bool canShoot = true;
    private Animator anim;
    [SerializeField] GameObject bulletPrefab;

    private void Awake()
    {
        anim = GetComponent<Animator>();
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
