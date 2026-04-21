using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float normalGravity = 3f;

    [Header("Inercia y Giro")]
    [SerializeField] private float glideAcceleration = 15f; 
    [SerializeField] private float glideDeceleration = 10f;

    [Header("Élitros")]
    [SerializeField] private float glideRotationSpeed = 120f;
    [SerializeField] private float maxGlideSpeed = 30f;
    [SerializeField] private float baseFallSpeed = 4f;
    [SerializeField] private float diveFallSpeed = 25f;
    [SerializeField] private float glideDrag = 1.0f;
    [SerializeField] private float climbPower = 1.6f;
    [SerializeField] private float climbResistance = 45f;

    [Header("Detección")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float fallLimit = -30f;

    private Rigidbody2D rb;
    private Player_Inputs controls;
    private Vector2 moveInput;
    private Vector3 initialScale;
    private bool isGrounded, isGliding;
    private float currentGlideAngle, currentGlideSpeed;
    private float currentHorizontalVel; 
    private float visualFlipX;          
    private Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        controls = new Player_Inputs();
        rb.gravityScale = normalGravity;
        initialScale = transform.localScale;
        visualFlipX = initialScale.x;
        controls.Player.Jump.performed += ctx => HandleJumpInput();
    }

    private void HandleJumpInput()
    {
        if (isGrounded) Jump();
        else StartGliding(!isGliding);
    }

    void Update()
    {
        moveInput = controls.Player.Move.ReadValue<Vector2>();
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        if (isGrounded && isGliding) StartGliding(false);

        if (transform.position.y < fallLimit)
        {
            ReiniciarNivel();
        }
        ActualizarAnimaciones();
    }

    private void ActualizarAnimaciones()
    {
        anim.SetFloat("Speed", Mathf.Abs(currentHorizontalVel));
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetBool("IsGliding", isGliding);
        anim.SetFloat("VerticalVel", rb.linearVelocity.y);
    }
    void FixedUpdate()
    {
        if (isGliding) ApplyGlidePhysics();
        else
        {
            float targetVel = moveInput.x * speed;
            currentHorizontalVel = Mathf.MoveTowards(currentHorizontalVel, targetVel, glideAcceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(currentHorizontalVel, rb.linearVelocity.y);
            
            HandleVisuals(0f);
        }
    }

    private void ApplyGlidePhysics()
    {
        currentGlideAngle += moveInput.y * glideRotationSpeed * Time.fixedDeltaTime;
        currentGlideAngle = Mathf.Clamp(currentGlideAngle, -60f, 60f);

        float pitchFactor = -Mathf.Sin(currentGlideAngle * Mathf.Deg2Rad);
        float speedChange = (pitchFactor < 0) ? pitchFactor * climbResistance : pitchFactor * 40f;
        currentGlideSpeed += (speedChange - glideDrag) * Time.fixedDeltaTime;
        currentGlideSpeed = Mathf.Clamp(currentGlideSpeed, 0.1f, maxGlideSpeed);

        float rad = currentGlideAngle * Mathf.Deg2Rad;
        float targetForward = moveInput.x * Mathf.Cos(rad) * currentGlideSpeed;
        
        float accelRate = (Mathf.Abs(moveInput.x) > 0.01f) ? glideAcceleration : glideDeceleration;
        currentHorizontalVel = Mathf.MoveTowards(currentHorizontalVel, targetForward, accelRate * Time.fixedDeltaTime);

        float liftEfficiency = Mathf.Clamp01(currentGlideSpeed / 10f);
        float lift = Mathf.Sin(rad) * currentGlideSpeed * liftEfficiency * climbPower;
        
        float currentMaxFall = (currentGlideAngle < 0) 
            ? Mathf.Lerp(baseFallSpeed, diveFallSpeed, Mathf.Abs(currentGlideAngle) / 60f) 
            : baseFallSpeed;

        float verticalVel = lift - 10f;
        float finalY = (verticalVel > 0) ? verticalVel : Mathf.Max(verticalVel, -currentMaxFall);

        rb.linearVelocity = new Vector2(currentHorizontalVel, finalY);

        HandleVisuals(currentGlideAngle);
    }

    private void HandleVisuals(float angle)
    {
        if (rb.linearVelocity.x > 0.01f)
        {
            transform.localScale = new Vector3(Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
        }
        else if (rb.linearVelocity.x < -0.01f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
        }

        transform.rotation = Quaternion.identity;
    }

    private void StartGliding(bool state)
    {
        isGliding = state;
        if (isGliding)
        {
            rb.gravityScale = 0;
            currentGlideAngle = -10f;
            currentGlideSpeed = Mathf.Max(rb.linearVelocity.magnitude, 8f);
        }
        else rb.gravityScale = normalGravity;
    }

    private void Jump()
    {
        StartGliding(false);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void ReiniciarNivel()
    {
        int escenaActual = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(escenaActual);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pincho"))
        {
            ReiniciarNivel();
        }
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();
}