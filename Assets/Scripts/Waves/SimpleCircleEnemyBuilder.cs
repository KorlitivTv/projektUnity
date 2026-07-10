using UnityEngine;

public class SimpleCircleEnemyBuilder : MonoBehaviour
{
    [SerializeField] private float scale = 0.45f;
    [SerializeField] private int sortingOrder = 100;
    [SerializeField] private string sortingLayerName = "Layer 3";

    private void Reset()
    {
        SetupEnemy();
    }

    [ContextMenu("Setup Simple Enemy")]
    public void SetupEnemy()
    {
        transform.localScale = new Vector3(scale, scale, 1f);

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingLayerName = sortingLayerName;
            spriteRenderer.sortingOrder = sortingOrder;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
            circleCollider.radius = 0.5f;
        }

        if (GetComponent<WaveEnemy>() == null)
        {
            gameObject.AddComponent<WaveEnemy>();
        }
    }
}
