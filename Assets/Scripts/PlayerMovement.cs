using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Run Parameters")]
    [SerializeField] private float _maxMovementSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _decceleration;

    [Header("Jump parameters")]
    [SerializeField] private float _jumpForce;

    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private float _groundCheckRadius;

    [Header("Layers and masks")]
    [SerializeField] private LayerMask _groundLayer;

    private Rigidbody2D _playerRB;
    private BoxCollider2D _boxCollider2D;
    private RopeSystem _playerRope;

    public bool IsGrounded { get; private set; }

    private void Awake()
    {
        _playerRB = GetComponent<Rigidbody2D>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _playerRope = GetComponent<RopeSystem>();
    }

    private void FixedUpdate()
    {
        if (_playerRope.isSwinging == false)
        {
            ApplyRun();
        }
        else
        {
            _playerRope.ApplySwingingForce();
            _playerRope.ClampRopeMovement();
        }

        CalculateCollisions();
        ApplyJump();
    }

    private void ApplyRun()
    {
        float targetSpeed = Input.GetAxisRaw("Horizontal") * _maxMovementSpeed; //Input in method
        float speedDif = targetSpeed - _playerRB.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _acceleration : _decceleration;
        float movement = speedDif * accelRate;
        _playerRB.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private void CalculateCollisions()
    {
        IsGrounded = Physics2D.OverlapCircle(_groundCheckPoint.position, _groundCheckRadius, _groundLayer);
        //IsFacingWall = Physics2D.OverlapCircle(_frontWallCheckPoint.position, _frontWallCheckRadius, _groundLayer);
    }

    private void ApplyJump()
    {
        if (IsGrounded && _playerRope.isSwinging == false && Input.GetKeyDown(KeyCode.Space))
        {
            _playerRB.velocity = new Vector2(_playerRB.velocity.x, 0);
            float force = _jumpForce;
            if (_playerRB.velocity.y < 0)
                force -= _playerRB.velocity.y;
            _playerRB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        }
    }

}
