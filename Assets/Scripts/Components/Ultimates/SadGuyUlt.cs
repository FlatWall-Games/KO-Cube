using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SadGuyUlt : AProjectile
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float projectileSize = 0.5f;
    HealthTankManager healthTank;

    int layerToIgnore;
    int layerMask;

    protected override void Awake()
    {
        base.Awake();
        layerToIgnore = 1 << 6;
        layerMask = ~layerToIgnore;
        SetTag(transform.parent.parent.tag);
        SetAttacker(transform.parent.parent.GetComponent<PlayerBehaviour>()); //Se fuerza la asignación del responsable del ataque en el awake, pues el rayo se lanza demasiado rápido.
        healthTank = transform.parent.parent.GetComponent<HealthTankManager>();
        RayShoot();
    }

    public override void CheckDestroy(Collider other) //Cada proyectil tiene sus condiciones de destrucción
    {
    }

    void RayShoot()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, projectileSize, transform.forward, out hit, Mathf.Infinity, layerMask))
        {
            HealthManager other = hit.transform.GetComponent<HealthManager>();
            if (other != null)
            {
                other.OnRaycastHit(this);

                if (!tag.Equals(other.tag))
                {
                    healthTank.UpdateHealthTank("damage", GetDamage());
                }
            }
            DrawRay(hit.point);
        }
    }

    void DrawRay(Vector3 hitPoint)
    {
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, hitPoint);
            StartCoroutine(HideRayAfterTime(0.1f));
        }
    }

    IEnumerator HideRayAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        lineRenderer.positionCount = 0; // Ocultar el rayo
    }
}
