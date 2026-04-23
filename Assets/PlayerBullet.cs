using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    public int damage = 10;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        damage = 10;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        DestroyIfOutOfScreen();
    }
     private void DestroyIfOutOfScreen()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        bool isOutOfScreen = viewportPos.x < 0f || viewportPos.x > 1f ||
                             viewportPos.y < 0f || viewportPos.y > 1f;

        if (isOutOfScreen)
            Destroy(gameObject);
    }
}

