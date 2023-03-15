using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthSystem : MonoBehaviour, IHealthSystem
{
    [SerializeField]
    private int _maxHealth;

    private int _health;

    private SceneService _sceneService = new SceneService();

    public int Health
    {
        get
        {
            return _health;
        }
    }

    public void Damage(int value)
    {
        _health = Mathf.Clamp(_health - Mathf.Abs(value), 0, _maxHealth);
        if(_health == 0)
        {
            Die();
        }    
    }

    public void Die()
    {
        _sceneService.Restart();
    }

    public void SetHealth(int value)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        _health = _maxHealth;
    }
}
