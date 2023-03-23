using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class RopeSystem : MonoBehaviour
{
    private bool _ropeAttached;
    private Rigidbody2D _ropeHingeAnchorRb;
    private SpriteRenderer _ropeHingeAnchorSprite;
    private List<Vector2> _ropePositions = new List<Vector2>();
    private Dictionary<Vector2, int> _wrapPointsLookup = new Dictionary<Vector2, int>();
    private Vector2 _ropeHook;
    private bool _distanceSet;
    public bool isSwinging;

    private Vector2 _playerPosition;
    private Rigidbody2D _playerRB;
    private SpriteRenderer _playerSprite;
    private float _horizontalInput;
    private float _verticalInput;
    private bool _isColliding;

    [SerializeField] private LineRenderer _ropeRenderer;
    [SerializeField] private LayerMask _ropeLayerMask;
    [SerializeField] private GameObject _ropeHingeAnchor;
    [SerializeField] private DistanceJoint2D _ropeJoint;
    [SerializeField] private float _ropeMaxCastDistance;
    [SerializeField] private float _climbSpeed;
    [SerializeField] private float _swingForce;
    [SerializeField] private Vector2 _maxSwingSpeed;

    [Space]

    [SerializeField] private Transform _crosshair;
    [SerializeField] private SpriteRenderer _crosshairSprite;

    private void Awake()
    {
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
            isSwinging = false;
            SetCrosshairPosition(aimAngle);
        }
        else
        {
            isSwinging = true;
            _ropeHook = _ropePositions.Last();
            _crosshairSprite.enabled = false;

            UpdateClosestRopePosition();
        }

        HandleInput(aimDirection);

        UpdateRopePositions();

        HandleRopeLength();

        HandleRopeUnwrap();
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

            if (_ropeAttached) return;
            _ropeRenderer.enabled = true;

            var hit = Physics2D.Raycast(_playerPosition, aimDirection, _ropeMaxCastDistance, _ropeLayerMask);

            if (hit.collider != null)
            {
                _ropeAttached = true;
                if (!_ropePositions.Contains(hit.point))
                {
                    transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 2f), ForceMode2D.Impulse);
                    _ropePositions.Add(hit.point);
                    _ropeJoint.distance = Vector2.Distance(_playerPosition, hit.point);
                    _ropeJoint.enabled = true;
                    _ropeHingeAnchorSprite.enabled = true;
                }
            }

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
        isSwinging = false;
        _ropeRenderer.positionCount = 2;
        _ropeRenderer.SetPosition(0, transform.position);
        _ropeRenderer.SetPosition(1, transform.position);
        _ropePositions.Clear();
        _wrapPointsLookup.Clear();
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

    private Vector2 GetClosestColliderPointFromRaycastHit(RaycastHit2D hit, PolygonCollider2D polyCollider)
    {
        var distanceDictionary = polyCollider.points.ToDictionary<Vector2, float, Vector2>(
            position => Vector2.Distance(hit.point, polyCollider.transform.TransformPoint(position)),
            position => polyCollider.transform.TransformPoint(position));

        var orderedDictionary = distanceDictionary.OrderBy(e => e.Key);
        return orderedDictionary.Any() ? orderedDictionary.First().Value : Vector2.zero;
    }

    private void UpdateClosestRopePosition()
    {
        //1
        if (_ropePositions.Count > 0)
        {
            // 2
            var lastRopePoint = _ropePositions.Last();
            var playerToCurrentNextHit = Physics2D.Raycast(_playerPosition, (lastRopePoint - _playerPosition).normalized, Vector2.Distance(_playerPosition, lastRopePoint) - 0.1f, _ropeLayerMask);

            // 3
            if (playerToCurrentNextHit)
            {
                var colliderWithVertices = playerToCurrentNextHit.collider as PolygonCollider2D;
                if (colliderWithVertices != null)
                {
                    var closestPointToHit = GetClosestColliderPointFromRaycastHit(playerToCurrentNextHit, colliderWithVertices);

                    // 4
                    if (_wrapPointsLookup.ContainsKey(closestPointToHit))
                    {
                        ResetRope();
                        return;
                    }

                    // 5
                    _ropePositions.Add(closestPointToHit);
                    _wrapPointsLookup.Add(closestPointToHit, 0);
                    _distanceSet = false;
                }
            }
        }
    }

    private void HandleRopeUnwrap()
    {
        if (_ropePositions.Count <= 1)
        {
            return;
        }

        // Hinge = следующая точка вверх от позиции игрока
        // Anchor = следующая точка вверх от Hinge
        // Hinge Angle = угол между anchor и hinge
        // Player Angle = угол между anchor и player

        var anchorIndex = _ropePositions.Count - 2;
        var hingeIndex = _ropePositions.Count - 1;
        var anchorPosition = _ropePositions[anchorIndex];
        var hingePosition = _ropePositions[hingeIndex];
        var hingeDir = hingePosition - anchorPosition;
        var hingeAngle = Vector2.Angle(anchorPosition, hingeDir);
        var playerDir = _playerPosition - anchorPosition;
        var playerAngle = Vector2.Angle(anchorPosition, playerDir);

        if (playerAngle < hingeAngle)
        {

            if (_wrapPointsLookup[hingePosition] == 1)
            {
                UnwrapRopePosition(anchorIndex, hingeIndex);
                return;
            }

            _wrapPointsLookup[hingePosition] = -1;
        }
        else
        {

            if (_wrapPointsLookup[hingePosition] == -1)
            {
                UnwrapRopePosition(anchorIndex, hingeIndex);
                return;
            }

            _wrapPointsLookup[hingePosition] = 1;
        }

    }

    private void UnwrapRopePosition(int anchorIndex, int hingeIndex)
    {
        var newAnchorPosition = _ropePositions[anchorIndex];
        _wrapPointsLookup.Remove(_ropePositions[hingeIndex]);
        _ropePositions.RemoveAt(hingeIndex);

        _ropeHingeAnchorRb.transform.position = newAnchorPosition;
        _distanceSet = false;

        if (_distanceSet)
        {
            return;
        }
        _ropeJoint.distance = Vector2.Distance(transform.position, newAnchorPosition);
        _distanceSet = true;
    }

    private void HandleRopeLength()
    {
        if (_ropeAttached && (_verticalInput != 0) && _isColliding == false)
            _ropeJoint.distance -= _verticalInput * _climbSpeed * Time.deltaTime;
    }

    //better in PlayerMovement
    public void GetInputVector()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
    }

    public void ApplySwingingForce()
    {
        if (_horizontalInput != 0)
        {
            _playerSprite.flipX = _horizontalInput > 0;

            if (isSwinging)
            {

                var playerToHookDirection = (_ropeHook - (Vector2)transform.position).normalized;

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

    public void ClampRopeMovement()
    {
        float xClamp = Mathf.Clamp(_playerRB.velocity.x, -_maxSwingSpeed.x, _maxSwingSpeed.x);
        float yClamp = Mathf.Clamp(_playerRB.velocity.y, -_maxSwingSpeed.y, _maxSwingSpeed.y);
        _playerRB.velocity = new Vector2(xClamp, yClamp);

    }


}