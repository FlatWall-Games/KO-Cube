using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;

public class PingController : NetworkBehaviour
{
    private Animator anim;

    private void Awake()
    {
        GameObject.FindObjectOfType<ModeSelector>().ModeSelected += OnModeSelected;
        anim = GetComponent<Animator>();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        StopAllCoroutines();
    }

    private void OnModeSelected()
    {
        StartCoroutine(CheckPing());
    }

    IEnumerator CheckPing()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
            EchoTimeStampServerRpc(Time.realtimeSinceStartup);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void EchoTimeStampServerRpc(float timeStamp, ServerRpcParams serverParams = default)
    {
        CalculateLagClientRpc(timeStamp,
        new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { serverParams.Receive.SenderClientId }
            }
        });
    }

    [ClientRpc]
    private void CalculateLagClientRpc(float timeStamp, ClientRpcParams clientParams)
    {
        float currentPing = (Time.realtimeSinceStartup - timeStamp) * 1000; //Se pasa a milisegundos 
        anim.SetBool("Lagged", currentPing >= 300);
    }
}
