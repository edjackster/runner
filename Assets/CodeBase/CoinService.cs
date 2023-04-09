using System;
using UnityEngine;
using UnityEngine.Events;

public class CoinService : IService
{
    [HideInInspector]
    public UnityEvent<int> onCoinsChange = new UnityEvent<int>();

    public int Coins
    {
        get { return _coins; }
        set
        {
            onCoinsChange.Invoke(value);
            _coins = value;
        }
    }

    private int _coins = 0;

    public void PicupACoin()
    {
        Coins++;
    }
}
