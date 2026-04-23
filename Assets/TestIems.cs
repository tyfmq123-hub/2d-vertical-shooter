using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class TestItem : MonoBehaviour
{
    public static TestItem  Instance;
    
    public ItemType itemType;
    
    public GameObject[] itemPrefabs;

    private void Awake()
    {
        Instance = this;
    }
    
    public enum ItemType
    {
        Coin,
        Boom,
        Power
    }
    

    public void CreateItem(Vector3 pos)
    {
        
        var prefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
        var go = Instantiate(prefab, pos, Quaternion.identity);
        var item3 = go.GetComponent<Item3>();
        StartCoroutine(item3.Move());
    }
}
