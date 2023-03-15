using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealthSystem
{
    public void Damage(int value);
    public void Die();
    public void SetHealth(int value);
}
