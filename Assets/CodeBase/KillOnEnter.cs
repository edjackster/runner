using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillOnEnter : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IHealthSystem healthSystem;
        if (collision.TryGetComponent<IHealthSystem>(out healthSystem))
        {
            healthSystem.Die();
        }
    }
}
