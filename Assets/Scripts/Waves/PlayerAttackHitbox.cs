using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float activeTime = 0.15f;
    [SerializeField] private KeyCode attackKey = KeyCode.Space;

    private Collider2D hitboxCollider;
    private float disableTime;

    private void Awake()
    {
        hitboxCollider = GetComponent<Collider2D>();
        hitboxCollider.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(attackKey))
        {
            hitboxCollider.enabled = true;
            disableTime = Time.time + activeTime;
        }

        if (hitboxCollider.enabled && Time.time >= disableTime)
        {
            hitboxCollider.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        WaveEnemy enemy = other.GetComponent<WaveEnemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }
}
