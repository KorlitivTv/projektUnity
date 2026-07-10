using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WaveEnemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int health = 2;
    [SerializeField] private int touchDamage = 1;
    [SerializeField] private float damageCooldown = 1f;

    private Transform player;
    private Rigidbody2D rb;
    private float nextDamageTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
        if (player == null)
        {
            return;
        }

        Vector2 direction = ((Vector2)player.position - rb.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            WaveSpawner.EnemyDefeated();
            Destroy(gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
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
            playerHealth.TakeDamage(touchDamage);
            nextDamageTime = Time.time + damageCooldown;
        }
    }
}
