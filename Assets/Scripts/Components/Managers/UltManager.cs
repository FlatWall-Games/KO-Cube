using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UltManager : MonoBehaviour
{
    [SerializeField] private float loadTime;
    [SerializeField] private Image ultBar;
    private bool canShoot = false;
    private float timer = 0;

    void Start()
    {
        ultBar.color = new Color(0.5f, 0.5f, 0.5f, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameObject.FindObjectOfType<AGameManager>().HasStarted()) return;
        if(timer < loadTime) timer += Time.deltaTime;
        else
        {
            ultBar.color = new Color(1, 1, 0, 1);
            canShoot = true;
        }
        ultBar.fillAmount = timer / loadTime;
    }

    public bool RequestShoot()
    {
        if (canShoot) return true;
        return false;
    }

    public void UpdateBar()
    {
        timer = 0;
        ultBar.fillAmount = 0;
        ultBar.color = new Color(0.5f, 0.5f, 0.5f, 1);
        canShoot = false;
    }

    public void HideBar()
    {
        transform.Find("Background").gameObject.SetActive(false);
    }
}
