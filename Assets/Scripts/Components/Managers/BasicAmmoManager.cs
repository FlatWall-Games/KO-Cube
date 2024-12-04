using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BasicAmmoManager : MonoBehaviour
{
    [SerializeField] private RectTransform singleBar; //Barra de munición
    [SerializeField] private int maxAmmo; //Munición máxima
    [SerializeField] private float reloadTime; //Tiempo para recargar
    [SerializeField] private AttackManager attackManager; //Script de disparo gestionado por este AmmoManager
    private float timer = 0;
    private int currentAmmo; //Munición actual
    private Image[] bars; //Todas las barras de munición
    
    void Awake()
    {
        bars = new Image[maxAmmo];
        bars[0] = singleBar.GetComponent<Image>();
        currentAmmo = maxAmmo;
        InitializeAmmoBars(); //Inicializa las barras de munición según la munición máxima
        transform.parent.GetComponent<HealthManager>().OnDead += HideBarOnDeath;
    }

    private void Update() //Gestiona la recarga según el ReloadTime
    {
        if(currentAmmo == 0 || attackManager.IsShooting()) GameObject.Find("BasicAttackIcon").GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 0.5f);
        else GameObject.Find("BasicAttackIcon").GetComponent<Image>().color = Color.white;
        if (currentAmmo == maxAmmo || attackManager.IsShooting()) return;
        timer += Time.deltaTime;
        bars[currentAmmo].fillAmount = Mathf.Clamp(timer / reloadTime, 0, 1);
        if (bars[currentAmmo].fillAmount == 1)
        {
            currentAmmo++;
            timer = 0;
        }
    }

    public bool ShootRequested() //Cuando se le da al botón de disparar esta función comprueba si se puede o no y da el visto bueno
    {
        if (attackManager.IsShooting() || currentAmmo <= 0) return false;
        return true;
    }

    public void UpdateAmmoBar()
    {
        float reloadedAmount = 0;
        if (currentAmmo != maxAmmo)
        {
            reloadedAmount = bars[currentAmmo].fillAmount;
            bars[currentAmmo].fillAmount = 0;
        }
        currentAmmo--;
        bars[currentAmmo].fillAmount = reloadedAmount;
    }

    private void InitializeAmmoBars()
    {
        singleBar.sizeDelta = new Vector2(singleBar.sizeDelta.x / maxAmmo, singleBar.sizeDelta.y); //Escalado de las barras
        float barOffset = singleBar.sizeDelta.x / 100 * 2.5f; //La distancia entre las barras es la longitud de la misma
        float initPos = 1 - barOffset/2; //Posición de la primera barra
        singleBar.anchoredPosition3D = new Vector3(singleBar.anchoredPosition3D.x + initPos, singleBar.anchoredPosition3D.y, singleBar.anchoredPosition3D.z);
        for (int i = 1; i < maxAmmo; i++) //Hay que contar que una barra ya existe
        {
            //Se instancian todas las barras en su posición correspondiente:
            RectTransform bar = GameObject.Instantiate(singleBar, singleBar.transform.parent);
            bar.anchoredPosition3D = new Vector3(bar.anchoredPosition3D.x - barOffset * i, bar.anchoredPosition3D.y, bar.anchoredPosition3D.z);
            bars[i] = bar.GetComponent<Image>(); //Se añade al array
        }
    }

    public void HideBars()
    {
        transform.Find("Background").gameObject.SetActive(false);
    }

    public void ResetAmmo()
    {
        foreach (Image bar in bars)
        {
            bar.fillAmount = 1;
        }
        timer = 0;
        currentAmmo = maxAmmo;
    }

    private void HideBarOnDeath(object s, string d)
    {
        transform.Find("Background").gameObject.SetActive(false);
    }

    public void RestoreBarOnSpawn()
    {
        transform.Find("Background").gameObject.SetActive(true);
    }
}
