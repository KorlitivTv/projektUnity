using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WaveEnemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Combat")]
    [SerializeField] private int health = 2;
    [SerializeField] private int touchDamage = 1;
    [SerializeField] private float damageCooldown = 1f;
    [SerializeField] private float deathDelay = 0.8f;

    [Header("Health Bar")]
    [SerializeField] private RectTransform healthBarFill;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float nextDamageTime;
    private bool isDead;
    private int currentHealth;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int HurtHash = Animator.StringToHash("Hurt");
    private static readonly int DieHash = Animator.StringToHash("Die");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = Mathf.Max(1, health);

        if (healthBarFill == null)
        {
            Transform fill = transform.Find("Canvas/Background/Fill");
            if (fill != null)
            {
                healthBarFill = fill.GetComponent<RectTransform>();
            }
        }

        UpdateHealthBar();
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void FixedUpdate()
    {
        if (isDead || player == null)
        {
            SetAnimationSpeed(0f);
            return;
        }

        Vector2 direction = ((Vector2)player.position - rb.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

        SetAnimationSpeed(direction.sqrMagnitude);
        UpdateFacingDirection(direction.x);
    }

    public void TakeDamage(int amount)
    {
        if (isDead || amount <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - amount);
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
            return;
        }

        if (animator != null)
        {
            animator.SetTrigger(HurtHash);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead || !collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        if (Time.time < nextDamageTime)
        {
            return;
        }

        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            if (animator != null)
            {
                animator.SetTrigger(AttackHash);
            }

            playerHealth.TakeDamage(touchDamage);
            nextDamageTime = Time.time + damageCooldown;
        }
    }

    private IEnumerator Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        Collider2D enemyCollider = GetComponent<Collider2D>();
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        if (animator != null)
        {
            animator.SetFloat(SpeedHash, 0f);
            animator.SetTrigger(DieHash);
        }

        WaveSpawner.EnemyDefeated();

        yield return new WaitForSeconds(deathDelay);
        Destroy(gameObject);
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill == null)
        {
            return;
        }

        float healthPercent = Mathf.Clamp01((float)currentHealth / Mathf.Max(1, health));
        Vector3 fillScale = healthBarFill.localScale;
        fillScale.x = healthPercent;
        healthBarFill.localScale = fillScale;
    }

    private void SetAnimationSpeed(float speed)
    {
        if (animator != null)
        {
            animator.SetFloat(SpeedHash, speed);
        }
    }

    private void UpdateFacingDirection(float horizontalDirection)
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (horizontalDirection > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (horizontalDirection < -0.01f)
        {
            spriteRenderer.flipX = true;
        }
    }
}
