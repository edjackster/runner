using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionAngle : MonoBehaviour
{
    private PlatformEffector2D _platformEffector;
    private BoxCollider2D _platformCollider;

    private float _colliderLength;
    private float _colliderHeight;
    private Vector2 _colliderCenter;
    private float _resultAngle;
    private Vector2 _leftTopPoint;
    private Vector2 _rightTopPoint;

    private void Awake()
    {
        _platformEffector = GetComponent<PlatformEffector2D>();
        _platformCollider = GetComponent<BoxCollider2D>();
    }
    void Start()
    {
        _colliderCenter = _platformCollider.offset;
        _colliderHeight = _platformCollider.size.y;
        _colliderLength = _platformCollider.size.x;

        Debug.Log(_platformCollider.offset); 
    }

}
