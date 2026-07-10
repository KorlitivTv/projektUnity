using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;

    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        GameUI.Instance?.SetHealth(currentHealth, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        GameUI.Instance?.SetHealth(currentHealth, maxHealth);
        Debug.Log("Player HP: " + currentHealth + "/" + maxHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Game Over");
            GameUI.Instance?.ShowGameOver();
            gameObject.SetActive(false);
        }
    }
}
