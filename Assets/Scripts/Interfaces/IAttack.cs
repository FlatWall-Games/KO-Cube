using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public interface IAttack
{
    public void CheckDestroy(Collider other);

    public float GetDamage();

    public float GetHealing();

    public string GetTag();

    public void SetTag(string tag);

    public void SetAttacker(PlayerBehaviour attacker);

    public PlayerBehaviour GetAttacker();

    public bool IsNetworkObject();

    public bool IsMele();
}
