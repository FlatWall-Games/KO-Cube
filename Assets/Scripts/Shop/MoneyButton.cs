using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MoneyButton : MonoBehaviour, IPurchaseButton
{
    [SerializeField] private int moneyAmount;
    [SerializeField] private GameObject animatedCoinPrefab;
    private Button button;
    private PurchaseWarning warning;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(GoToPurchaseWarning); //Me daba pereza asignarlo desde el inspector
        warning = PurchaseWarning.Instance;
    }

    private void GoToPurchaseWarning()
    {
        warning.SetButton(this);
        warning.gameObject.SetActive(true);
    }

    public void Purchase()
    {
        Debug.Log($"Compradas {moneyAmount} monedas");
        PlayerDataManager.Instance.AddCoins(moneyAmount);
        MoneyText.Instance.UpdateMoney();
        GameObject.FindObjectOfType<SFXManager>().PlaySFX(2, 1);
        StartCoroutine(InstantiateCoins());
    }

    public string GetButtonType()
    {
        return "Money";
    }    

    IEnumerator InstantiateCoins()
    {
        int numCoins = (int)Mathf.Clamp(Mathf.Sqrt(moneyAmount) / 4, 5, 30);
        for(int i = 0; i < numCoins; i++)
        {
            GameObject.Instantiate(animatedCoinPrefab, this.gameObject.GetComponent<RectTransform>());
            yield return new WaitForSeconds(0.05f);
        }
    }
}
