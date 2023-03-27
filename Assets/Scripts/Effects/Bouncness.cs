using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncness : MonoBehaviour
{
    [Header("Настройки упругости")]
    [SerializeField] private Transform _playerRigidbody;
    [SerializeField] private Transform _playerSprite;
    [SerializeField] private Vector3 _squishDown = new Vector3(1.15f, 0.85f, 1f);
    [SerializeField] private Vector3 _squishUp = new Vector3(0.85f, 1.15f, 1f);

    [Header("Сила упругости")]
    [Range(0f, 1f)]
    [SerializeField] private float _squishForce;

    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
        Vector2 localPosition = _playerRigidbody.InverseTransformPoint(transform.position);
        
        var t = localPosition.y * _squishForce;
        Debug.Log(t);
        Vector3 scale = Lerp3(_squishUp, Vector3.one, _squishDown, t);
        
        _playerSprite.localScale = scale;
    }

    private Vector3 Lerp3(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        if (t < 0f)
            return Vector3.Lerp(b, a, Mathf.Abs(t));
        else
            return Vector3.Lerp(b, c, t);
    }
}
