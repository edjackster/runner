using Assets.CodeBase.Infrastructure;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Run Parameters")]
    [SerializeField] private float _maxMovementSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _decceleration;

    [Space]

    [Header("Jump parameters")]
    [SerializeField] private float _jumpForce;
    [SerializeField] private Vector2 _wallJumpingForce;
    [SerializeField] private float _wallJumpTime;
    [SerializeField] private int _extraJumpsValue;    
    private int _extraJumps;

    [Space]

    [Header("Apex modifires")]
    [SerializeField] private float _apexInterval;
    [SerializeField] private float _gravityModifier;
    private Vector2 _basicGravity;

    [Space]

    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private Transform _frontWallCheckPoint;
    [SerializeField] private float _frontWallCheckRadius;
    public bool isSwinging;

    [Space]

    [Header("Layers and masks")]
    [SerializeField] private LayerMask _groundLayer;

    private InputService _inputService;
    private Rigidbody2D _playerRB;
    private BoxCollider2D _boxCollider2D;
    public Vector2 ropeHook;
    [SerializeField] private float swingForce = 4f;

    [SerializeField] private float _wallSlidingSpeed;
    

    //Checks
    public bool IsJumping { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool IsFacingRight { get; private set; }
    public bool IsFacingWall { get; private set; }
    public bool IsWallJumping { get; private set; }
    public bool IsWallSliding { get; private set; }

    private void Awake()
    {
        _playerRB = GetComponent<Rigidbody2D>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        
    }

    private void Start()
    {
        _inputService = new InputService(); //AllServices.Instance.GetService<InputService>();
        _basicGravity = Physics2D.gravity;
        _extraJumps = _extraJumpsValue;
        IsFacingRight = true;
    }

    private void Update()
    {
        CalculateCollisions();

        ResetExtraJumps();

        //if (IsFacingWall == true && IsGrounded == false && _inputService.GetInputVector().x != 0)
        //    IsWallSliding = true;
        //else
        //    IsWallSliding = false;

        //if (IsWallSliding == true)
        //    ApplyWallSliding();

        if (_inputService.GetJumpButtonDown() && IsGrounded == true && isSwinging == false)
        {
            ApplyJump();
        }
        else if (_inputService.GetJumpButtonDown() && isSwinging == false && IsGrounded == false && _extraJumps > 0)
        {
            ApplyJump();
            _extraJumps--;
        }

        //ApplyWallJump();
        CutJump();
    }

    private void FixedUpdate()
    {
        if (isSwinging== true)
            ApplySwingForce();
        else
            ApplyRun();

        if (IsFacingRight == false && _playerRB.velocity.x > 0)
            FlipPlayer();
        else if (IsFacingRight == true && _playerRB.velocity.x < 0)
            FlipPlayer();
    }

    private void CalculateCollisions()
    {
        IsGrounded = Physics2D.OverlapCircle(_groundCheckPoint.position, _groundCheckRadius, _groundLayer);
        IsFacingWall = Physics2D.OverlapCircle(_frontWallCheckPoint.position, _frontWallCheckRadius, _groundLayer);
    }

    private void ApplyRun()
    {
        if (IsWallJumping == false)
        {
            float targetSpeed = _inputService.GetInputVector().x * _maxMovementSpeed;
            float speedDif = targetSpeed - _playerRB.velocity.x;
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _acceleration : _decceleration;
            float movement = speedDif * accelRate;
            _playerRB.AddForce(movement * Vector2.right, ForceMode2D.Force);
        }
            
    }

    private void FlipPlayer()
    {
        IsFacingRight = !IsFacingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y);
    }

    private void ResetExtraJumps()
    {
        if (IsGrounded == true || IsWallSliding == true)
            _extraJumps = _extraJumpsValue;
    }

    private void ApplyJump()
    {
        IsJumping = true;
        _playerRB.velocity = new Vector2 (_playerRB.velocity.x, 0);
        float force = _jumpForce;
        if (_playerRB.velocity.y <0)
            force -= _playerRB.velocity.y;
        _playerRB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    private void ApplyWallJump()
    {
        if (_inputService.GetJumpButtonDown() && IsWallSliding)
        {
            IsWallJumping = true;
            IsJumping = true;
            FlipPlayer();

            _playerRB.velocity = new Vector2(_playerRB.velocity.x, 0);
            Invoke("SetWallJumpToFalse", _wallJumpTime);

            Vector2 force = _wallJumpingForce;
            Vector2 jumpDir = _inputService.GetInputVector().normalized * -1;
            if (_playerRB.velocity.y < 0)
                force.y -= _playerRB.velocity.y;
            if (IsWallJumping)
                _playerRB.AddForce(new Vector2(force.x * jumpDir.x, force.y), ForceMode2D.Impulse);
        }
        
    }

    private void CutJump()
    {
        if (IsJumping == true && _inputService.GetJumpButtonUp() == true && _playerRB.velocity.y > 0)
        {
            IsJumping = false;
            _playerRB.velocity = new Vector2(_playerRB.velocity.x, 0);
        }
    }
    
    private void ApplyWallSliding()
        =>_playerRB.velocity = new Vector2(_playerRB.velocity.x, Mathf.Clamp(_playerRB.velocity.y, -_wallSlidingSpeed, float.MaxValue));

    private void SetWallJumpToFalse()
        => IsWallJumping = false;

    private void ApplySwingForce()
    {
        var playerToHookDirection = (ropeHook - (Vector2)transform.position).normalized;

        // 2 - »нвертируем направление, чтобы получить перпендикул€рное направление
        Vector2 perpendicularDirection;
        if (_inputService.GetInputVector().x < 0)
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

        var force = perpendicularDirection * swingForce;
        _playerRB.AddForce(force, ForceMode2D.Force);
    }
}
    