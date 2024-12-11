using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationForcer : MonoBehaviour
{
    public string animationName;
    public float time;
    public int layer;
    public float speed = 1;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Animator>().speed = speed;
            GetComponent<Animator>().Play(animationName, layer, time);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject.FindWithTag("MainCamera").GetComponent<Animator>().Rebind();
        }
    }
}
