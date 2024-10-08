using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraSetup : NetworkBehaviour
{
    CinemachineVirtualCamera camera;
    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) return;

        camera = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        camera.Follow = transform;
        camera.LookAt = transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
