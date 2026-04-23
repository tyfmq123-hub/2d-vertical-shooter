using UnityEngine;
using System.Collections;

public class Item3 : MonoBehaviour
{
    public enum ItemType
    {
        Coin, Boom, Power
    }

    public ItemType itemType;

    public float speed = 2f;

    public IEnumerator Move()
    {
        while (true)
        {
            if (this == null || gameObject == null)
                yield break;

            transform.Translate(Vector3.down * speed * Time.deltaTime);

            if (transform.position.y <= -5.5f)
            {
                Destroy(gameObject);
                yield break;
            }

            yield return null;
        }
    }
}