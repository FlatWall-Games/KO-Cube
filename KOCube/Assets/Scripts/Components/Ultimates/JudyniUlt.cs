using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudyniUlt : AProjectile
{
    
        [SerializeField] float rayDistance;
        [SerializeField] float offset;
        private bool hasAttacked = false; // Para controlar si el ataque ya se ejecutó

        protected override void Awake()
        {
            base.Awake();
            Transform parent = transform.parent;
        }

        private void Update()
        {
            // Si attacker está configurado y aún no ha ejecutado tpRaycast
            if (this.attacker != null && !hasAttacked)
            {
                Debug.Log("Raycast lanzado");
                tpRaycast();
                hasAttacked = true; // Evita llamar a tpRaycast múltiples veces
            }
        }

        public void tpRaycast()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, rayDistance))
            {
                Vector3 teleportPosition = hit.point + hit.normal * offset;

                // Teletransporta al jugador a la posición ajustada
                attacker.TeleportPlayerServerRpc(teleportPosition);

                Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);
            }
            else
            {
                Vector3 targetPosition = transform.position + transform.forward * rayDistance;
                attacker.TeleportPlayerServerRpc(targetPosition);
            }
        }


    public override void CheckDestroy(Collider other) // Cada proyectil tiene sus condiciones de destrucción
        {
        }
}
