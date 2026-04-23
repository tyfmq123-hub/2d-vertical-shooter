using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Item3 item = other.GetComponent<Item3>();

        if (item != null)
        {
            switch (item.itemType)
            {
                case Item3.ItemType.Coin:
                    Debug.Log("코인 먹음!");
                    break;

                case Item3.ItemType.Boom:
                    Debug.Log("폭탄 먹음!");
                    break;

                case Item3.ItemType.Power:
                    Debug.Log("파워업 먹음!");
                    break;
            }

            Destroy(other.gameObject);
        }
    }

    void Update()
    {
       
    }
}