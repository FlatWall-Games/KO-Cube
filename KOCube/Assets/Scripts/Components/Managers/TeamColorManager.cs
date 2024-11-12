using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamColorManager : MonoBehaviour
{
    private Light colorLight;

    void Awake()
    {
        colorLight = GetComponent<Light>();
    }

    public void SetColor(Color color)
    {
        colorLight.color = color;
    }
}
