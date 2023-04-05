using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI CoinCounter;
    public  int Coins
    {
        get { return _coins; }
        set
        {
            CoinCounter.text = value.ToString();
            _coins = value;
        }
    }

    private int _coins = 0;

    public void PicupACoin()
    {
        Coins++;
    }
}
