using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MachinganUlt : AProjectile
{
    private float health = 300;
    [SerializeField] private Renderer shieldRenderer;
    [SerializeField] private Material redMaterial;

    protected override void Awake()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        base.Awake();
        rb.velocity = this.transform.forward * speed;
        this.transform.parent = null; //Se desvincula del padre para que no le afecte su movimiento
    }

    private void Start()
    {
        if (PlayerBehaviour.ownerTag != this.tag) shieldRenderer.material = redMaterial;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (collision.gameObject.CompareTag("Floor"))
        {
            GetComponent<Rigidbody>().isKinematic = true;
            tag = "Untagged";
        }
    }

    public override void CheckDestroy(Collider other) //Cada proyectil tiene sus condiciones de destrucción
    {
        if (!NetworkManager.Singleton.IsServer) return;

        IAttack projectile = other.GetComponent<IAttack>();
        if (projectile != null) health -= projectile.GetDamage();
        if (health <= 0) Destroy(this.gameObject);
    }
}
