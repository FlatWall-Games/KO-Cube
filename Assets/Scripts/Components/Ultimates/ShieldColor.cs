using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldColor : MonoBehaviour
{
    [SerializeField] private GameObject enemyShield;
    private AttackManager attackManager;

    private void Awake()
    {
        attackManager = GetComponent<AttackManager>();
        StartCoroutine(GetTag());
    }

    IEnumerator GetTag()
    {
        while(tag.Equals("Untagged")) yield return new WaitForSeconds(0.01f);
        if (tag.Equals(PlayerBehaviour.ownerTag)) attackManager.SetUlt(enemyShield);
    }
}
