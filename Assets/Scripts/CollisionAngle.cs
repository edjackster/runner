using UnityEngine;

public class CollisionAngle : MonoBehaviour
{
    private PlatformEffector2D _platformEffector;
    private BoxCollider2D _platformCollider;

    private float _colliderLength;
    private float _colliderHeight;
    private Vector2 _colliderCenter;
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
        _colliderHeight = Camera.main.WorldToScreenPoint(_platformCollider.size).y;
        _colliderLength = Camera.main.WorldToScreenPoint(_platformCollider.size).x;

        _leftTopPoint = new Vector2(_colliderCenter.x - _colliderLength, _colliderCenter.y + _colliderHeight);
        _rightTopPoint = new Vector2(_colliderCenter.x + _colliderLength, _colliderCenter.y + _colliderHeight);


        _platformEffector.surfaceArc = Vector2.Angle(_leftTopPoint, _rightTopPoint);
        Debug.Log(_platformCollider.offset); 
    }

}
