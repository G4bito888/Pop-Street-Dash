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
    [SerializeField] private float glideAcceleration = 15f; // Qué tan rápido gana velocidad horizontal
    [SerializeField] private float glideDeceleration = 10f; // Qué tan rápido se frena al soltar

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
    private float currentHorizontalVel; // Velocidad horizontal suavizada
    private float visualFlipX;           // Valor de escala X suavizado

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
    }

    void FixedUpdate()
    {
        if (isGliding) ApplyGlidePhysics();
        else
        {
            // Movimiento terrestre con aceleración básica
            float targetVel = moveInput.x * speed;
            currentHorizontalVel = Mathf.MoveTowards(currentHorizontalVel, targetVel, glideAcceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(currentHorizontalVel, rb.linearVelocity.y);
            
            // Visuales básicos
            HandleVisuals(0f);
        }
    }

    private void ApplyGlidePhysics()
    {
        // 1. Ángulo y Energía
        currentGlideAngle += moveInput.y * glideRotationSpeed * Time.fixedDeltaTime;
        currentGlideAngle = Mathf.Clamp(currentGlideAngle, -60f, 60f);

        float pitchFactor = -Mathf.Sin(currentGlideAngle * Mathf.Deg2Rad);
        float speedChange = (pitchFactor < 0) ? pitchFactor * climbResistance : pitchFactor * 40f;
        currentGlideSpeed += (speedChange - glideDrag) * Time.fixedDeltaTime;
        currentGlideSpeed = Mathf.Clamp(currentGlideSpeed, 0.1f, maxGlideSpeed);

        // 2. Inercia Horizontal (Aceleración gradual)
        float rad = currentGlideAngle * Mathf.Deg2Rad;
        float targetForward = moveInput.x * Mathf.Cos(rad) * currentGlideSpeed;
        
        // Determinar si usamos aceleración o frenado
        float accelRate = (Mathf.Abs(moveInput.x) > 0.01f) ? glideAcceleration : glideDeceleration;
        currentHorizontalVel = Mathf.MoveTowards(currentHorizontalVel, targetForward, accelRate * Time.fixedDeltaTime);

        // 3. Sustentación y Gravedad
        float liftEfficiency = Mathf.Clamp01(currentGlideSpeed / 10f);
        float lift = Mathf.Sin(rad) * currentGlideSpeed * liftEfficiency * climbPower;
        
        float currentMaxFall = (currentGlideAngle < 0) 
            ? Mathf.Lerp(baseFallSpeed, diveFallSpeed, Mathf.Abs(currentGlideAngle) / 60f) 
            : baseFallSpeed;

        float verticalVel = lift - 10f;
        float finalY = (verticalVel > 0) ? verticalVel : Mathf.Max(verticalVel, -currentMaxFall);

        rb.linearVelocity = new Vector2(currentHorizontalVel, finalY);

        // 4. Visuales Suavizados
        HandleVisuals(currentGlideAngle);
    }

    private void HandleVisuals(float angle)
{
    // 1. Flip instantáneo: Solo cambiamos el signo si hay movimiento significativo
    // Esto evita que el personaje pase por el valor "0" de escala
    if (rb.linearVelocity.x > 0.01f)
    {
        transform.localScale = new Vector3(Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
    }
    else if (rb.linearVelocity.x < -0.01f)
    {
        transform.localScale = new Vector3(-Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
    }

    // 2. Mantenerlo recto siempre (tu petición anterior)
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
        // Obtiene el índice de la escena actual y la vuelve a cargar
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