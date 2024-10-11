using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public interface IAttack
{
    public void CheckDestroy(string otherTag);

    public float GetDamage();

    public float GetHealing();

    public string GetTag();

    public void SetTag(string tag);

    public void SetAttacker(PlayerMovement attacker);

    public PlayerMovement GetAttacker();
}
