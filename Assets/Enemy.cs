using UnityEngine;

public class Enemy : MonoBehaviour
{
    private SpriteRenderer sr;
    private Test uiManager;
    private Player player;
    public GameObject bulletPrefab;
    public Transform firePoint;
    
    public int health;
    public Sprite[] sprites;
    public float speed = 2f;
    public bool canShoot = true;

    private float fireDelay = 1.5f;
    private float timer = 0f;
    private Vector3 moveDirection = Vector3.down;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        uiManager = FindFirstObjectByType<Test>();
        player = FindFirstObjectByType<Player>();

        if (sr != null)
        {
            sr.sortingOrder = Mathf.Max(sr.sortingOrder, 10);
        }

        if (gameObject.name.StartsWith("Enemy A") || gameObject.name.StartsWith("Enemy B"))
        {
            canShoot = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!Mathf.Approximately(transform.position.z, 0f))
        {
            Vector3 p = transform.position;
            p.z = 0f;
            transform.position = p;
        }

        transform.position += moveDirection * (speed * Time.deltaTime);
        
        timer += Time.deltaTime;
        if (canShoot && timer > fireDelay)
        {
            Shoot();
            timer = 0;
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null)
        {
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab);
        Transform shootPoint = firePoint != null ? firePoint : transform;
        bullet.transform.position = shootPoint.position;

        if (gameObject.name.StartsWith("Enemy C"))
        {
            if (player == null)
            {
                player = FindFirstObjectByType<Player>();
            }

            Vector2 targetDir = Vector2.down;
            if (player != null)
            {
                Vector3 toPlayer = player.transform.position - shootPoint.position;
                targetDir = new Vector2(toPlayer.x, toPlayer.y).normalized;
            }

            bullet.transform.rotation = Quaternion.FromToRotation(Vector2.down, targetDir);

            EnemyBullet enemyBullet = bullet.GetComponent<EnemyBullet>();
            if (enemyBullet == null)
            {
                enemyBullet = bullet.GetComponentInChildren<EnemyBullet>();
            }

            if (enemyBullet != null)
            {
                enemyBullet.SetDirection(targetDir);
            }
        }
    }
    public void StartMove(Vector3 dir)
    {
        moveDirection = dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector3.down;
    }

    private void Hit(int damage)
    {
        health -= damage;
        sr.sprite = sprites[1];
        Invoke("ReturnDefaultSprite", 0.1f);

        if (health <= 0)
        {
            AddKillScore();
            Destroy(gameObject);
        }
    }

    private void AddKillScore()
    {
        if (uiManager == null)
        {
            return;
        }

        string enemyName = gameObject.name;
        int scoreToAdd = 0;

        if (enemyName.StartsWith("Enemy A"))
        {
            scoreToAdd = 100;
        }
        else if (enemyName.StartsWith("Enemy B"))
        {
            scoreToAdd = 200;
        }
        else if (enemyName.StartsWith("Enemy C"))
        {
            scoreToAdd = 300;
        }

        if (scoreToAdd > 0)
        {
            uiManager.AddScore(scoreToAdd);
        }
    }

    private void ReturnDefaultSprite()
    {
        sr.sprite = sprites[0];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerBullet"))
        {
            PlayerBullet playerBullet = other.gameObject.GetComponent<PlayerBullet>();
            Hit(playerBullet.damage);
            
            Destroy(other.gameObject);
        }
    }
}
