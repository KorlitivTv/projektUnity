using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PenguinPlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode attackKey = KeyCode.Space;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;
    private bool isSprinting;
    private bool isAttacking;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int SprintHash = Animator.StringToHash("Sprint");
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        ReadInput();
        UpdateVisualDirection();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void ReadInput()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        isSprinting = Input.GetKey(sprintKey) && moveInput != Vector2.zero;

        if (Input.GetKeyDown(attackKey))
        {
            animator.SetTrigger(AttackHash);
        }
    }

    private void Move()
    {
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        rb.MovePosition(rb.position + moveInput * currentSpeed * Time.fixedDeltaTime);
    }

    private void UpdateVisualDirection()
    {
        if (moveInput.x > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput.x < -0.01f)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void UpdateAnimator()
    {
        animator.SetFloat(SpeedHash, moveInput.sqrMagnitude);
        animator.SetBool(SprintHash, isSprinting);
    }
}
