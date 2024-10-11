using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IUI
{
    IState State { get; set; }
    GameObject Canvas { get; set; }
}
