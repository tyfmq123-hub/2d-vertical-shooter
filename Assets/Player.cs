using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public Transform firePoint;
    public GameObject playerBulletPrefab1;
    public GameObject playerBulletPrefab2;
    private float moveSpeed = 5f;
    public float gap = 0.05f;
    public float fireRate = 0.2f;
    private float nextFireTime = 0f;
    public int power = 1;
    public int hp = 100;
    public Test uiManager;
    public float respawnDelay = 1.5f;

    private float _fireTimer;

    private Vector2 _spriteExtents;
    private Vector3 _spawnPosition;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _playerCollider;
    private bool _isRespawning;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerCollider = GetComponent<Collider2D>();
        _spawnPosition = transform.position;

        if (_spriteRenderer != null)
            _spriteExtents = _spriteRenderer.bounds.extents;
    }

    void Update()
    {
        if (_isRespawning)
        {
            return;
        }

        Move();

        if (Input.GetKey(KeyCode.Space))
        {
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isRespawning || !other.CompareTag("EnemyBullet"))
        {
            return;
        }

        Destroy(other.gameObject);

        bool isGameOver = false;

        if (uiManager != null)
        {
            isGameOver = uiManager.LoseLife();
        }
        else
        {
            EnemyBullet bullet = other.GetComponent<EnemyBullet>();
            if (bullet != null)
            {
                hp -= bullet.damage;
            }

            isGameOver = hp <= 0;
        }

        // 피격 시에는 우선 비활성화하고, 게임오버가 아니면 리스폰 코루틴으로 복귀시킨다.
        if (isGameOver)
        {
            Destroy(gameObject);
            return;
        }

        StartCoroutine(RespawnCoroutine());
    }

    private void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(h, v, 0).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);

        Vector3 minBounds = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 maxBounds = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minBounds.x + _spriteExtents.x, maxBounds.x - _spriteExtents.x);
        pos.y = Mathf.Clamp(pos.y, minBounds.y + _spriteExtents.y, maxBounds.y - _spriteExtents.y);
        transform.position = pos;
    }

    private void Shoot()
    {
        switch (power)
        {
            case 1:
                SpawnBullet(playerBulletPrefab2, Vector3.zero);
                break;

            case 2:
                SpawnBullet(playerBulletPrefab2, Vector3.left * gap);
                SpawnBullet(playerBulletPrefab2, Vector3.right * gap);
                break;

            case 3:
                SpawnBullet(playerBulletPrefab1, Vector3.zero);
                SpawnBullet(playerBulletPrefab2, Vector3.left * gap);
                SpawnBullet(playerBulletPrefab2, Vector3.right * gap);
                break;
        }
    }

    private void SpawnBullet(GameObject prefab, Vector3 offset)
    {
        Instantiate(prefab, firePoint.position + offset, firePoint.rotation);
    }

    private IEnumerator RespawnCoroutine()
    {
        _isRespawning = true;
        SetPlayerVisible(false);

        yield return new WaitForSeconds(respawnDelay);

        transform.position = _spawnPosition;
        SetPlayerVisible(true);
        _isRespawning = false;
    }

    private void SetPlayerVisible(bool isVisible)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.enabled = isVisible;
        }

        if (_playerCollider != null)
        {
            _playerCollider.enabled = isVisible;
        }
    }
}
