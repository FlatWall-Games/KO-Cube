using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlraUlt : AProjectile
{
    //Escala inicial de la esfera (del prefab)
    [SerializeField] float inicialScale = 0.1f;
    //Escala maxima que alcanzara la esfera
    [SerializeField] float maxScale = 20f;

    float scaleRatio;
    protected override void Awake()
    {
        base.Awake();
        //Calculamos la diferencia de escala de la esfera
        float scaleChange = maxScale - inicialScale;
        //Calculamos cuanto hay que escalar la esfera por segundo aplicando (maxScale - initialScale) / duracion del escalada (en nuestro caso, el tiempo que dura viva la esfera)
        scaleRatio = scaleChange / this.timeToDestroy;
        Debug.Log("Esfera creada");
    }

    protected void Update()
    {
        transform.localScale += new Vector3(scaleRatio, scaleRatio, scaleRatio) * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    public override void CheckDestroy(string otherTag) //Cada proyectil tiene sus condiciones de destrucción
    {
       
    }
}
