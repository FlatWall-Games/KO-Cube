using System.Collections;
using System;
using UnityEngine;

public class PurchaseWarning : MonoBehaviour
{
    public static PurchaseWarning Instance;
    private IPurchaseButton currentPurchaseButton;
    [SerializeField] private GameObject videoPlayer;

    private void Awake()
    {
        Instance = this;
    }

    private void Start() //En el Awake los botones le referencian así que lo apago en el Start.
    {
        this.gameObject.SetActive(false);
    }

    public void SetButton(IPurchaseButton button)
    {
        currentPurchaseButton = button;
    }

    public void Purchase()
    {
        if (currentPurchaseButton.GetButtonType().Equals("Money")) StartCoroutine(PurchaseAfterVideo());
        else
        {
            currentPurchaseButton.Purchase();
            this.gameObject.SetActive(false);
        }
    }

    IEnumerator PurchaseAfterVideo()
    {
        videoPlayer.SetActive(true);
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 5.0f));
        videoPlayer.SetActive(false);
        currentPurchaseButton.Purchase();
        this.gameObject.SetActive(false);
    }
}
