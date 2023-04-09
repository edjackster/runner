using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CoinCollector coinCollector;

        if (collision.TryGetComponent(out coinCollector) == false) return;

        coinCollector.PicupACoin();
        Destroy(gameObject);
    }
}
