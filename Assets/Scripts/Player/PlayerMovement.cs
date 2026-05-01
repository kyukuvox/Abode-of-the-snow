using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
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

    [Header("Sons")]
    public AudioClip walkSound;
    public float walkSoundInterval = 0.3f;
    public AudioSource walkAudioSource; 

    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;
    private float walkSoundTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (walkAudioSource == null)
        {
            walkAudioSource = gameObject.AddComponent<AudioSource>();
            walkAudioSource.playOnAwake = false;
            walkAudioSource.loop = false;
        }
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
        if (DialogueManager.Instance.IsActive())
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        if (GameStateManager.Instance.IsCinematicMode())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (ItemDescriptionManager.Instance.IsActive())
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

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
        if (DialogueManager.Instance.IsActive())
        {
            if (animator != null) animator.SetBool("isWalking", false);
            StopWalkSound();
            return;
        }

        if (MenuManager.Instance.IsMenuOpen())
        {
            if (animator != null) animator.SetBool("isWalking", false);
            StopWalkSound();
            return;
        }

        if (PauseMenu.Instance.IsPaused())
        {
            if (animator != null) animator.SetBool("isWalking", false);
            StopWalkSound();
            return;
        }

        if (GameStateManager.Instance.IsCinematicMode())
        {
            if (animator != null) animator.SetBool("isWalking", false);
            StopWalkSound();
            return;
        }

        if (ItemDescriptionManager.Instance.IsActive())
        {
            if (animator != null) animator.SetBool("isWalking", false);
            StopWalkSound();
            return;
        }

        horizontalInput = Input.GetAxisRaw("Horizontal");
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded)
            jumpRequested = true;

        bool isMoving = horizontalInput != 0 && isGrounded;

        if (animator != null)
            animator.SetBool("isWalking", horizontalInput != 0);

        if (isMoving)
            PlayWalkSound();
        else
            StopWalkSound();

        if (horizontalInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (horizontalInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void PlayWalkSound()
    {
        if (walkSound == null) return;

        walkSoundTimer -= Time.deltaTime;
        if (walkSoundTimer <= 0f)
        {
            walkAudioSource.PlayOneShot(walkSound);
            walkSoundTimer = walkSoundInterval;
        }
    }

    void StopWalkSound()
    {
        walkSoundTimer = 0f;
        if (walkAudioSource.isPlaying)
            walkAudioSource.Stop();
    }

    private float horizontalInput;
    private bool isGrounded;
    private bool jumpRequested;
}