using UnityEngine;
using Random = UnityEngine.Random;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    [Header("Drop Settings")]
    [Range(0f, 1f)]
    public float dropChance = 0.45f;
    public GameObject[] itemPrefabs;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureInstanceExists()
    {
        if (Instance != null)
        {
            return;
        }

        ItemManager existing = FindFirstObjectByType<ItemManager>();
        if (existing != null)
        {
            Instance = existing;
            return;
        }

        GameObject managerGo = new GameObject("ItemManager");
        Instance = managerGo.AddComponent<ItemManager>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SpawnItem(Vector3 worldPosition)
    {
        if (itemPrefabs == null || itemPrefabs.Length == 0)
        {
            return;
        }

        GameObject prefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
        if (prefab == null)
        {
            return;
        }

        GameObject itemGo = Instantiate(prefab, worldPosition, Quaternion.identity);
        Item3 item = itemGo.GetComponent<Item3>();
        if (item == null)
        {
            item = itemGo.GetComponentInChildren<Item3>();
        }

        if (item != null)
        {
            item.BeginMove();
        }
    }

    public void TrySpawnItem(Vector3 worldPosition)
    {
        if (Random.value <= dropChance)
        {
            SpawnItem(worldPosition);
        }
    }
}
