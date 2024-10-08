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
        RotatePlayer(lookRotation);
        anim.SetTrigger("Shoot");
    }

    private void RotatePlayer(Quaternion lookRotation)
    {
        transform.rotation = lookRotation;
    }

    public void ShootSingleBullet()
    {
        GameObject.Instantiate(bulletPrefab, transform);
    }

    public bool IsShooting()
    {
        return shooting;
    }

    public void OnShootEnded() //Llamado desde la animación
    {
        canShoot = true;
        shooting = false;
    }
}
