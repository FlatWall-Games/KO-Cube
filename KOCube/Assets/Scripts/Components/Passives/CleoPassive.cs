using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleoPassive : MonoBehaviour
{
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private Transform basicTransform;
    private float counter = 0;

    public void AddHit()
    {
        if(++counter == 3)
        {
            counter = -1;
            IAttack attack = GameObject.Instantiate(attackPrefab, basicTransform).GetComponent<IAttack>();
            attack.SetTag(this.tag);
            attack.SetAttacker(GetComponent<PlayerBehaviour>());
        }
    }

    public void ResetCounter()
    {
        counter = 0;
    }
}
