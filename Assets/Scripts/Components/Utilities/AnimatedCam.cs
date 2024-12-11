using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AnimatedCam : MonoBehaviour
{
    [SerializeField] private float rotationSpeedX = 100;
    [SerializeField] private float rotationSpeedY = 50;
    [SerializeField] private float speed = 50;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private CinemachineTrackedDolly cart;

    private void Awake()
    {
        cart = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    void Update()
    {
        //ManuallyMove();
        if (Input.GetKeyDown(KeyCode.R)) cart.m_PathPosition = 0; //Con la R se resetea
        cart.m_PathPosition += speed * Time.deltaTime;
    }

    private void ManuallyMove()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        float rotationX = 0;
        float rotationY = 0;

        if (Input.GetKey(KeyCode.W))
        {
            rotationX = -rotationSpeedX * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            rotationX = rotationSpeedX * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            rotationY = -rotationSpeedY * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            rotationY = rotationSpeedY * Time.deltaTime;
        }

        transform.Rotate(rotationX, rotationY, 0);
    }
}
