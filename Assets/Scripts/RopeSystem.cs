using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class RopeSystem : MonoBehaviour
{
    private bool _ropeAttached;
    private Rigidbody2D _ropeHingeAnchorRb;
    private SpriteRenderer _ropeHingeAnchorSprite;
    private List<Vector2> _ropePositions = new List<Vector2>();
    private Vector2 _ropeHook;
    private bool _distanceSet;
    private bool _isSwinging;

    private Vector2 _playerPosition;
    private Rigidbody2D _playerRB;
    private SpriteRenderer _playerSprite;
    private float _horizontalInput;
    private float _verticalInput;

    [SerializeField] private LineRenderer _ropeRenderer;
    [SerializeField] private LayerMask _ropeLayerMask;
    [SerializeField] private GameObject _ropeHingeAnchor;
    [SerializeField] private DistanceJoint2D _ropeJoint;
    [SerializeField] private float _ropeMaxCastDistance;
    [SerializeField] private float _climbSpeed;
    [SerializeField] private Vector2 _maxSwingForce;
    [SerializeField] private float _swingForce;

    [Space]

    [SerializeField] private Transform _crosshair;
    [SerializeField] private SpriteRenderer _crosshairSprite;

    private bool _isStarted;


    private void Awake()
    {
        _isStarted = false;
        _ropeJoint.enabled = false;
        _playerPosition = transform.position;
        _playerRB = GetComponent<Rigidbody2D>();
        _playerSprite = GetComponent<SpriteRenderer>();
        _ropeHingeAnchorRb = _ropeHingeAnchor.GetComponent<Rigidbody2D>();
        _ropeHingeAnchorSprite = _ropeHingeAnchor.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        GetInputVector();
        var worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        var facingDirection = worldMousePosition - transform.position;
        var aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
        if (aimAngle < 0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }

        var aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;

        _playerPosition = transform.position;

        if (!_ropeAttached)
        {
            _isSwinging = false;
            SetCrosshairPosition(aimAngle);
        }
        else
        {
            _isSwinging = true;
            _ropeHook = _ropePositions.Last();
            _crosshairSprite.enabled = false;
        }

        HandleInput(aimDirection);

        UpdateRopePositions();

        HandleRopeLength();
    }


    private void FixedUpdate()
    {
        BeginGame();
        ApplySwingingForce();
    }

    private void GetInputVector()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void SetCrosshairPosition(float aimAngle)
    {
        if (!_crosshairSprite.enabled)
        {
            _crosshairSprite.enabled = true;
        }

        var x = transform.position.x + 1f * Mathf.Cos(aimAngle);
        var y = transform.position.y + 1f * Mathf.Sin(aimAngle);

        var crossHairPosition = new Vector3(x, y, 0);
        _crosshair.transform.position = crossHairPosition;
    }

    private void HandleInput(Vector2 aimDirection)
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 2
            if (_ropeAttached) return;
            _ropeRenderer.enabled = true;

            var hit = Physics2D.Raycast(_playerPosition, aimDirection, _ropeMaxCastDistance, _ropeLayerMask);

            // 3
            if (hit.collider != null)
            {
                _ropeAttached = true;
                _isStarted = true;
                if (!_ropePositions.Contains(hit.point))
                {
                    // 4
                    // Немного подпрыгивает над землёй, когда игрок к чему-то прицепится крюком.
                    transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 2f), ForceMode2D.Impulse);
                    _ropePositions.Add(hit.point);
                    _ropeJoint.distance = Vector2.Distance(_playerPosition, hit.point);
                    _ropeJoint.enabled = true;
                    _ropeHingeAnchorSprite.enabled = true;
                }
            }
            // 5
            else
            {
                _ropeRenderer.enabled = false;
                _ropeAttached = false;
                _ropeJoint.enabled = false;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            ResetRope();
        }
    }

    private void ResetRope()
    {
        _ropeJoint.enabled = false;
        _ropeAttached = false;
        _isSwinging = false;
        _ropeRenderer.positionCount = 2;
        _ropeRenderer.SetPosition(0, transform.position);
        _ropeRenderer.SetPosition(1, transform.position);
        _ropePositions.Clear();
        _ropeHingeAnchorSprite.enabled = false;
    }

    private void UpdateRopePositions()
    {

        if (!_ropeAttached)
        {
            return;
        }

        _ropeRenderer.positionCount = _ropePositions.Count + 1;

        for (var i = _ropeRenderer.positionCount - 1; i >= 0; i--)
        {
            if (i != _ropeRenderer.positionCount - 1)
            {
                _ropeRenderer.SetPosition(i, _ropePositions[i]);

                // 4
                if (i == _ropePositions.Count - 1 || _ropePositions.Count == 1)
                {
                    var ropePosition = _ropePositions[_ropePositions.Count - 1];
                    if (_ropePositions.Count == 1)
                    {
                        _ropeHingeAnchorRb.transform.position = ropePosition;
                        if (!_distanceSet)
                        {
                            _ropeJoint.distance = Vector2.Distance(transform.position, ropePosition);
                            _distanceSet = true;
                        }
                    }
                    else
                    {
                        _ropeHingeAnchorRb.transform.position = ropePosition;
                        if (!_distanceSet)
                        {
                            _ropeJoint.distance = Vector2.Distance(transform.position, ropePosition);
                            _distanceSet = true;
                        }
                    }
                }
                
                else if (i - 1 == _ropePositions.IndexOf(_ropePositions.Last()))
                {
                    var ropePosition = _ropePositions.Last();
                    _ropeHingeAnchorRb.transform.position = ropePosition;
                    if (!_distanceSet)
                    {
                        _ropeJoint.distance = Vector2.Distance(transform.position, ropePosition);
                        _distanceSet = true;
                    }
                }
            }
            else
            {
                _ropeRenderer.SetPosition(i, transform.position);
            }
        }
    }

    private void HandleRopeLength()
    {
        if (_ropeAttached && (Input.GetAxis("Vertical") != 0))
            _ropeJoint.distance -= _verticalInput * _climbSpeed * Time.deltaTime;
    }

    private void ApplySwingingForce()
    {
        if (_horizontalInput!=0)
        {
            _playerSprite.flipX = _horizontalInput > 0;

            if (_isSwinging)
            {
                
                // 1 - получаем нормализованный вектор направления от игрока к точке крюка
                var playerToHookDirection = (_ropeHook - (Vector2)transform.position).normalized;

                // 2 - Инвертируем направление, чтобы получить перпендикулярное направление
                Vector2 perpendicularDirection;
                if (_horizontalInput < 0)
                {
                    perpendicularDirection = new Vector2(-playerToHookDirection.y, playerToHookDirection.x);
                    var leftPerpPos = (Vector2)transform.position - perpendicularDirection * -2f;
                    Debug.DrawLine(transform.position, leftPerpPos, Color.green, 0f);
                }
                else
                {
                    perpendicularDirection = new Vector2(playerToHookDirection.y, -playerToHookDirection.x);
                    var rightPerpPos = (Vector2)transform.position + perpendicularDirection * 2f;
                    Debug.DrawLine(transform.position, rightPerpPos, Color.green, 0f);
                }

                var force = perpendicularDirection * _swingForce;
                _playerRB.AddForce(force, ForceMode2D.Force);
            }
        }
    }

    private void BeginGame()
    {
        if (_isStarted == false)
            _playerRB.velocity = Vector2.zero;
    }


}
