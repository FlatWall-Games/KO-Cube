using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyText : MonoBehaviour
{
    public static MoneyText Instance;
    void Awake()
    {
        Instance = this;
    }

    public void UpdateMoney()
    {
        GetComponent<TextMeshProUGUI>().text = PlayerDataManager.Instance.GetCoins().ToString();
    }
}
