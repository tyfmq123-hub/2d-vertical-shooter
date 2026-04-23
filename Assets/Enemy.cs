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
    public int debugSortingOrder = 10;
    public bool forceVisibleForDebug = true;

    private float fireDelay = 1.5f;
    private float timer = 0f;
    private Vector3 moveDirection = Vector3.down;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        uiManager = FindObjectOfType<Test>();
        player = FindObjectOfType<Player>();

        // 배경/타일맵에 가려지는 경우를 피하기 위해 적 스프라이트 정렬 우선순위를 올림
        if (sr != null)
        {
            if (forceVisibleForDebug)
            {
                sr.enabled = true;
                sr.color = new Color(1f, 1f, 1f, 1f);
                sr.sortingLayerName = "Default";
            }
            sr.sortingOrder = Mathf.Max(sr.sortingOrder, debugSortingOrder);
        }

        if (forceVisibleForDebug)
        {
            // 너무 작거나 0 스케일이면 보이지 않으므로 최소 크기 보장
            Vector3 s = transform.localScale;
            if (Mathf.Abs(s.x) < 0.8f || Mathf.Abs(s.y) < 0.8f)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }

        // 생성 시 이름이 Enemy A(Clone)처럼 붙어도 발사 비활성화되도록 처리
        if (gameObject.name.StartsWith("Enemy A") || gameObject.name.StartsWith("Enemy B"))
        {
            canShoot = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 2D 카메라 기준에서 z가 틀어지면 안 보일 수 있으므로 0으로 고정
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

        // Enemy C만 플레이어를 조준해 발사
        if (gameObject.name.StartsWith("Enemy C"))
        {
            if (player == null)
            {
                player = FindObjectOfType<Player>();
            }

            Vector2 targetDir = Vector2.down;
            if (player != null)
            {
                Vector3 toPlayer = player.transform.position - shootPoint.position;
                targetDir = new Vector2(toPlayer.x, toPlayer.y).normalized;
            }

            // 탄 프리팹 내부 이동 스크립트가 local down을 쓸 수도 있어서 회전도 함께 맞춤
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
