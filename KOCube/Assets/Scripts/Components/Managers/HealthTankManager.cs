using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthTankManager : NetworkBehaviour
{
    [SerializeField] Image tankBar;
    [SerializeField] float maxEnergy = 600;
    float _energy = 0;

    public float Energy { get { return _energy; } set { _energy = value; } }
    public float MaxEnergy { get { return _energy; } }

    private void Start()
    {
        if (!IsOwner)
        {
            tankBar.transform.parent.gameObject.SetActive(false);
        }

        if (IsServer)
        {
            GetComponent<HealthManager>().OnDead += OnPlayerDead;
        }
    }

    public void UpdateHealthTank(string type, float amount)
    {
        if (!IsServer) return;

        if (type == "damage")
        {
            _energy += amount;
            if (_energy > maxEnergy) _energy = maxEnergy;
        }
        else
        {
            _energy -= amount;
            if (_energy <= 0) _energy = 0;
        }

        UpdateHealthTankClientRpc(_energy);
    }

    [ClientRpc]
    void UpdateHealthTankClientRpc(float newEnergy)
    {
        _energy = newEnergy;
        float value = _energy / maxEnergy;
        tankBar.fillAmount = value;
    }

    void OnPlayerDead()
    {
        _energy = 0;
        UpdateHealthTankClientRpc(_energy);
    }
}
