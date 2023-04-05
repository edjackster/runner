using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CoinService coinService = AllServices.Instance.GetService<CoinService>();
        coinService.PicupACoin();
        Destroy(gameObject);
    }
}
