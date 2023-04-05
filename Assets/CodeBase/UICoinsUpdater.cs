using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UICoinsUpdater : MonoBehaviour
{
    private TextMeshProUGUI _coinstext;
    private CoinService _coinService;
    
    // Start is called before the first frame update
    void Start()
    {
        CoinService coinService = AllServices.Instance.GetService<CoinService>();
        coinService.onCoinsChange.AddListener(UpdateCoins);
        _coinstext = GetComponent<TextMeshProUGUI>();
    }

    void UpdateCoins(int value)
    {
        _coinstext.text = value.ToString();
    }
}
