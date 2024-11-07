using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPurchaseButton
{
    public void Purchase();

    public string GetButtonType();
}
