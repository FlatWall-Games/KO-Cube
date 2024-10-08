using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float timeToDestroy;
    [SerializeField] private float damage;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = this.transform.forward * speed;
        this.transform.parent = null;
        Destroy(this.gameObject, timeToDestroy);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player")) Destroy(this.gameObject);
    }
}
