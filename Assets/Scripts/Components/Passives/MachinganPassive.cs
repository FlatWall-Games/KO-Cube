using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Playables;

public class MachinganPassive : NetworkBehaviour
{
    [SerializeField] private float speedTime;
    [SerializeField] private float speedMultiplier;
    private float baseSpeed;
    private PlayerBehaviour playerBehaviour;

    void Awake()
    {
        playerBehaviour = GetComponent<PlayerBehaviour>();
        baseSpeed = playerBehaviour.speed;
    }

    public void StartSpeedWindow()
    {
        if (IsServer)
        {
            StopAllCoroutines();
            StartCoroutine(SpeedWindow());
        }
    }

    IEnumerator SpeedWindow()
    {
        playerBehaviour.speed = baseSpeed*speedMultiplier;
        yield return new WaitForSeconds(speedTime);
        playerBehaviour.speed = baseSpeed;
    }
}
