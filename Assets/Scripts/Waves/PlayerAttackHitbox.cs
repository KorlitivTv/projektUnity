using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float activeTime = 0.15f;
    [SerializeField] private KeyCode attackKey = KeyCode.Space;

    [Header("Audio")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField, Range(0f, 1f)] private float attackVolume = 1f;

    private Collider2D hitboxCollider;
    private AudioSource audioSource;
    private float disableTime;

    private void Awake()
    {
        hitboxCollider = GetComponent<Collider2D>();

        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = false;
        }

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = GetComponentInParent<AudioSource>();
        }
    }

    private void Update()
    {
        if (Time.timeScale <= 0f)
        {
            if (hitboxCollider != null)
            {
                hitboxCollider.enabled = false;
            }

            return;
        }

        if (Input.GetKeyDown(attackKey))
        {
            if (hitboxCollider != null)
            {
                hitboxCollider.enabled = true;
                disableTime = Time.time + activeTime;
            }

            PlayAttackSound();
        }

        if (hitboxCollider != null && hitboxCollider.enabled && Time.time >= disableTime)
        {
            hitboxCollider.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.timeScale <= 0f)
        {
            return;
        }

        WaveEnemy enemy = other.GetComponent<WaveEnemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }

    private void PlayAttackSound()
    {
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound, attackVolume);
        }
    }
}