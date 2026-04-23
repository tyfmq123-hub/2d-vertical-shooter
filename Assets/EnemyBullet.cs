using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 3f;

    public int damage = 10;
    private Vector2 moveDirection = Vector2.down;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
        
        if (Camera.main != null)
        {
            Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
            bool isOutOfScreen = viewportPos.x < -0.1f || viewportPos.x > 1.1f || viewportPos.y < -0.1f || viewportPos.y > 1.1f;
            if (isOutOfScreen)
            {
                Destroy(gameObject);
            }
        }
        else if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    public void SetDirection(Vector2 dir)
    {
        moveDirection = dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector2.down;
    }
}
