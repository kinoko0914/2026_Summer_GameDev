using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private float speed;
    private Vector2 direction;

    public void Launch(Vector2 launchDirection, float bulletSpeed)
    {
        direction = launchDirection.normalized;
        speed = bulletSpeed;

        Destroy(gameObject, 5f);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
            }

            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}