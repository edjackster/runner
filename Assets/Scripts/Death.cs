using System;
using UnityEngine;

public class Death : MonoBehaviour
{
    public static Action onDeath;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        onDeath?.Invoke();
    }
}
