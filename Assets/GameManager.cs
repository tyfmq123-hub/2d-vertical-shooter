using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public enum SpawnMode
    {
        Mixed,
        TopOnly,
        SideOnly
    }

    public GameObject[] enemies;
    public Transform[] spawnPoints; //위에서 아래로 내려오는 위치 
    public EnemySpawner[]  spawners;    //사이드 위치 
    public SpawnMode spawnMode = SpawnMode.Mixed;
    public bool prioritizeTopSpawnInMixed = true;

    private float delta = 0;
    private int span = 0; 

    void Start()
    {
        AutoCollectTopSpawnPointsIfEmpty();
        span = Random.Range(1, 4);
    }
    
    void Update()
    {
        delta += Time.deltaTime;
        
        if (delta > span)
        {
            //만들어라 
            CreateEnemy();
            delta = 0;
        
            span = Random.Range(1, 4);  //1, 2, 3
        }
    }

    private void CreateEnemy()
    {
        if (enemies == null || enemies.Length == 0) return;

        GameObject prefab = enemies[Random.Range(0, enemies.Length)];
        if (prefab == null) return;

        Transform topPoint = GetRandomTopSpawnPoint();
        EnemySpawner sideSpawner = GetRandomSideSpawner();

        bool canTop = topPoint != null;
        bool canSide = sideSpawner != null;
        if (!canTop && !canSide) return;

        int dice;
        if (spawnMode == SpawnMode.TopOnly && canTop)
        {
            dice = 0;
        }
        else if (spawnMode == SpawnMode.SideOnly && canSide)
        {
            dice = 1;
        }
        else
        {
            if (canTop && canSide)
            {
                dice = prioritizeTopSpawnInMixed ? 0 : Random.Range(0, 2);
            }
            else
            {
                dice = canTop ? 0 : 1;
            }
        }
        //만약에 0 이라면 위에서 아래로 내려오는거고 
        //1 이라면 사이드 위치를 잡아야 함 

        GameObject enemyGo = Instantiate(prefab);
        var enemy = enemyGo.GetComponent<Enemy>();
        
        if (dice == 0)
        {
            enemyGo.transform.position = topPoint.position;
            if (enemy != null) enemy.StartMove(Vector2.down);
        }
        else
        {
            enemyGo.transform.position = sideSpawner.startPoint.position;
            Vector3 sideDir = sideSpawner.GetDir().normalized;
            if (enemy != null) enemy.StartMove(sideDir);
        }
    }

    private Transform GetRandomTopSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return null;

        int startIndex = Random.Range(0, spawnPoints.Length);
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            int idx = (startIndex + i) % spawnPoints.Length;
            if (spawnPoints[idx] != null) return spawnPoints[idx];
        }

        return null;
    }

    private void AutoCollectTopSpawnPointsIfEmpty()
    {
        if (spawnPoints != null && spawnPoints.Length > 0) return;

        Transform[] allTransforms = FindObjectsByType<Transform>(FindObjectsSortMode.None);
        List<Transform> collected = new List<Transform>();

        for (int i = 0; i < allTransforms.Length; i++)
        {
            Transform t = allTransforms[i];
            if (t != null && t.name.StartsWith("SpawnPoint"))
            {
                collected.Add(t);
            }
        }

        if (collected.Count > 0)
        {
            spawnPoints = collected.ToArray();
        }
    }

    private EnemySpawner GetRandomSideSpawner()
    {
        if (spawners == null || spawners.Length == 0) return null;

        int startIndex = Random.Range(0, spawners.Length);
        for (int i = 0; i < spawners.Length; i++)
        {
            int idx = (startIndex + i) % spawners.Length;
            EnemySpawner candidate = spawners[idx];
            if (candidate != null && candidate.startPoint != null) return candidate;
        }

        return null;
    }
}
