using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class AlraProjectile : AProjectile
{
    //Angulo maximo de desviacion de las balas en grados
    float maxAngleDeviation = 10f;
    protected override void Awake()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        base.Awake();
        //Calculamos un angulo de desviacion entre -maxAngleDeviation y maxAngleDeviation
        float randomAngle = Random.Range(-maxAngleDeviation, maxAngleDeviation);
        //Generamos un eje aleatorio de longitud 1 para aplicar la rotacion
        Vector3 ramdonAxis = Random.onUnitSphere;
        //Creamos y guardamos la rotacion entorno al eje obtenido con los grados de rotacion obtenidos antes
        Quaternion rotation = Quaternion.AngleAxis(randomAngle, ramdonAxis);
        //Aplicamos la rotacion al eje up que, es la direccion de la bala sin desviar
        Vector3 bulletDirection = rotation * this.transform.up;

        rb.velocity = bulletDirection * speed;
        this.transform.parent = null; //Se desvincula del padre para que no le afecte su movimiento
    }


    public override void CheckDestroy(Collider other) //Cada proyectil tiene sus condiciones de destrucción
    {
        if (!NetworkManager.Singleton.IsServer) return;

        string otherTag = other.tag;
        Debug.Log("choqué con algo");
        //En este caso, el proyectil se destruye al chocar con un jugador del otro equipo o con un objeto del mapa
        if (otherTag.Equals("Team1"))
        {
            if (this.tag.Equals("Team2")) Destroy(this.gameObject);
        }
        else if (otherTag.Equals("Team2"))
        {
            if (this.tag.Equals("Team1")) Destroy(this.gameObject);
        }
        else Destroy(this.gameObject);
    }
}
