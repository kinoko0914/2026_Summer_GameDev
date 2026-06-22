using UnityEngine;

public class TurretEnemy : MonoBehaviour
{
    [Header("設定用のオブジェクト")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;  

    [Header("砲台のステータス")]
    [SerializeField] private float fireInterval; 
    [SerializeField] private float bulletSpeed;   
    [SerializeField] private float attackRange; 

    private Transform playerTransform;
    private float timer;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= attackRange)
        {
            timer += Time.deltaTime;

            if (timer >= fireInterval)
            {
                Shoot();
                timer = 0f;
            }
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        Vector2 targetDirection = (Vector2)playerTransform.position - (Vector2)firePoint.position;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        EnemyBullet bullet = bulletObj.GetComponent<EnemyBullet>();
        if (bullet != null)
        {
            bullet.Launch(targetDirection, bulletSpeed);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}