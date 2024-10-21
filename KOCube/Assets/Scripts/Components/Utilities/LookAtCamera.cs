using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform cameraTransform;

    void Awake()
    {
        cameraTransform = GameObject.FindWithTag("MainCamera").transform;
    }

    void Update()
    {
        this.transform.forward = cameraTransform.position - this.transform.position;
    }
}
