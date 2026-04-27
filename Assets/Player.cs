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
    public int coinCount;
    public Test uiManager;
    public float respawnDelay = 1.5f;
    public GameObject boomEffectPrefab;
    public float boomEffectDuration = 2f;
    
    public int bombCount = 0;
    public int maxBomb = 3;

    private float _fireTimer;

    private Vector2 _spriteExtents;
    private Vector3 _spawnPosition;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _playerCollider;
    private bool _isRespawning;
    private bool _isBombActive;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerCollider = GetComponent<Collider2D>();
        _spawnPosition = transform.position;
        if (uiManager == null)
        {
            uiManager = FindFirstObjectByType<Test>();
        }

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

        if (Input.GetMouseButtonDown(1))
        {
            UseBomb();
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Item3 item = other.GetComponent<Item3>();
        if (item != null)
        {
            CollectItem(item);
            return;
        }

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

    private void CollectItem(Item3 item)
    {
        switch (item.itemType)
        {
            case Item3.ItemType.Coin:
                coinCount++;
                if (uiManager != null)
                {
                    uiManager.AddScore(200);
                }
                break;
            case Item3.ItemType.Boom:
                AddBomb(1);
                break;
            case Item3.ItemType.Power:
                power = Mathf.Clamp(power + 1, 1, 3);
                break;
        }

        Destroy(item.gameObject);
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

    public void AddBomb(int amount)
    {
        bombCount += amount;
        bombCount = Mathf.Clamp(bombCount, 0, maxBomb);
    }

    private void UseBomb()
    {
        if (_isBombActive || bombCount <= 0)
        {
            return;
        }

        bombCount--;
        StartCoroutine(BoomSkillCoroutine());
    }

    private IEnumerator BoomSkillCoroutine()
    {
        _isBombActive = true;

        // 폭탄 발동 "처음 1회"에만 전체 적/적탄 제거.
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                enemies[i].KillByBomb();
            }
        }

        EnemyBullet[] enemyBullets = FindObjectsByType<EnemyBullet>(FindObjectsSortMode.None);
        for (int i = 0; i < enemyBullets.Length; i++)
        {
            if (enemyBullets[i] != null)
            {
                Destroy(enemyBullets[i].gameObject);
            }
        }

        if (boomEffectPrefab != null)
        {
            Vector3 spawnPos = Vector3.zero;
            if (Camera.main != null)
            {
                spawnPos = Camera.main.transform.position;
            }
            spawnPos.z = 0f;

            GameObject fx = Instantiate(boomEffectPrefab, spawnPos, Quaternion.identity);
            SpriteRenderer[] renderers = fx.GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].sortingOrder = Mathf.Max(renderers[i].sortingOrder, 200);
            }

            Destroy(fx, boomEffectDuration);
        }

        yield return new WaitForSeconds(boomEffectDuration);
        _isBombActive = false;
    }

}
