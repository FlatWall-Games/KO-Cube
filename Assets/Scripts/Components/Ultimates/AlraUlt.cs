using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlraUlt : AProjectile
{
    //Escala inicial de la esfera (del prefab)
    [SerializeField] float inicialScale = 0.1f;
    //Escala maxima que alcanzara la esfera
    [SerializeField] float maxScale = 20f;

    [SerializeField] float healingInterval;

    float scaleRatio;
    protected override void Awake()
    {
        base.Awake();
        //Calculamos la diferencia de escala de la esfera
        float scaleChange = maxScale - inicialScale;
        //Calculamos cuanto hay que escalar la esfera por segundo aplicando (maxScale - initialScale) / duracion del escalada (en nuestro caso, el tiempo que dura viva la esfera)
        scaleRatio = (scaleChange / this.timeToDestroy) * 4;

        StartCoroutine(ResetTrigger());
    }

    public override void SetAttacker(PlayerBehaviour attacker)
    {
        base.SetAttacker(attacker);
        this.attacker.GetComponent<HealthManager>().OnDead += DestroyOnDead;
    }

    protected void Update()
    {
        if (transform.localScale.x < maxScale)
        {
            transform.localScale += new Vector3(scaleRatio, 0, scaleRatio) * Time.deltaTime;
        }
        if(transform.localScale.x > maxScale)
        {
            transform.localScale = new Vector3(maxScale, 1, maxScale);
        }
    }

    IEnumerator ResetTrigger()
    {
        while (true)
        {
            yield return new WaitForSeconds(healingInterval);
            GetComponent<Collider>().enabled = false;
            GetComponent<Collider>().enabled = true;
        }
    }

    private void DestroyOnDead(object s, string t)
    {
        this.attacker.GetComponent<HealthManager>().OnDead -= DestroyOnDead;
        Destroy(this.gameObject);
    }
}
