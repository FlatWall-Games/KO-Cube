using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedCoin : MonoBehaviour
{
    private Vector2 targetPosition;
    private RectTransform rectTransform;
    private bool animate = false;
    [SerializeField] private float speed;
    
    void Awake()
    {
        speed = Random.Range(3, 6);
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(Random.Range(-50, 50), Random.Range(-50,50));
        targetPosition = new Vector2(766, 393);
        this.transform.parent = GameObject.Find("Shop").transform;
        Invoke("StartAnimationAfterCooldown", Random.Range(0.05f, 0.2f));
    }

    void Update()
    {
        if(animate) rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, speed*Time.deltaTime);
        if (Vector2.Distance(rectTransform.anchoredPosition, targetPosition) < 100f) Destroy(this.gameObject);
    }

    private void StartAnimationAfterCooldown()
    {
        animate = true;
    }
}


