using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationForcer : MonoBehaviour
{
    public string animationName;
    public float time;
    public int layer;
    public float speed = 1;

    void Start()
    {
        GetComponent<Animator>().speed = speed;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Animator>().Play(animationName, layer, time);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject.FindWithTag("MainCamera").GetComponent<Animator>().Rebind();
        }
    }
}
