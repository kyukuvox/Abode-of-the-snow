using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float groundAcceleration = 15f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float airAcceleration = 4f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Dash")]
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 5f;

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;
    private bool jumpRequested;

    private bool isDashing;
    private bool dashRequested;
    private float dashTimer;
    private float cooldownTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
    }

    void FixedUpdate()
    {
        JumpPhysic();
    }

    void JumpPhysic()
    {
        // dash
        if (dashRequested)
        {
            isDashing = true;
            dashTimer = dashDuration;
            cooldownTimer = dashCooldown;
            dashRequested = false;

            float dashDir = horizontalInput != 0 ? horizontalInput : transform.localScale.x;
            rb.linearVelocity = new Vector2(dashDir * dashForce, 0f);
        }

        if (isDashing)
        {
            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
                isDashing = false;
            return;
        }

        // cooldown dash
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.fixedDeltaTime;

        float targetSpeed = horizontalInput * moveSpeed;
        float acceleration = isGrounded ? groundAcceleration : airAcceleration;
        float newX = Mathf.Lerp(rb.linearVelocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);

        if (jumpRequested)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpRequested = false;
        }
    }

    void Move()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded)
            jumpRequested = true;

        if (Input.GetKeyDown(KeyCode.C) && cooldownTimer <= 0f && !isDashing)
            dashRequested = true;

        // Tourne le sprite
        if (horizontalInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (horizontalInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }
}