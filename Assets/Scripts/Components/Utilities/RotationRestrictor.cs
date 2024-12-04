using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationRestrictor : MonoBehaviour
{
    private Quaternion initialRotation;
    
    void Awake()
    {
        initialRotation = transform.rotation;    
    }

    void Update()
    {
        transform.rotation = initialRotation;
    }
}
