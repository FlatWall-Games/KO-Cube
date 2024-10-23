using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SadGuyUlt : AProjectile
{
    [SerializeField] LineRenderer lineRenderer;

    protected override void Awake()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        base.Awake();
        SetAttacker(transform.parent.parent.GetComponent<PlayerBehaviour>()); //Se fuerza la asignación del responsable del ataque en el awake, pues el rayo se lanza demasiado rápido.
        RayShoot();
    }

    public override void CheckDestroy(Collider other) //Cada proyectil tiene sus condiciones de destrucción
    {
        if (!NetworkManager.Singleton.IsServer) return;

    }

    void RayShoot()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            HealthManager health = hit.transform.GetComponent<HealthManager>();
            if (health != null)
            {
                health.OnRaycastHit(this);
            }
            DrawRay(hit.point);
        }
    }

    void DrawRay(Vector3 hitPoint)
    {
        if (!NetworkManager.Singleton.IsServer) return;

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
