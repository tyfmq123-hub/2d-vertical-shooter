using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class LoadMain : MonoBehaviour
{
    [Header("Stage Data")]
    public TextAsset stageDataJson;

    [Header("Enemy Prefabs")]
    public GameObject enemyAPrefab;
    public GameObject enemyBPrefab;
    public GameObject enemyCPrefab;

    // stage_data.json의 point 인덱스에 대응하는 스폰 위치 배열
    public Transform[] spawnPoints;

    void Start()
    {
        AutoCollectSpawnPointsIfEmpty();

        TextAsset jsonAsset = stageDataJson != null
            ? stageDataJson
            : Resources.Load<TextAsset>("stage_data");

        if (jsonAsset == null)
        {
            Debug.LogError("LoadMain: stage_data.json을 찾을 수 없습니다. stageDataJson을 직접 할당하거나 Resources/stage_data에 배치하세요.");
            return;
        }

        SpawnData[] arr = JsonConvert.DeserializeObject<SpawnData[]>(jsonAsset.text);
        if (arr == null || arr.Length == 0)
        {
            Debug.LogWarning("LoadMain: 스폰 데이터가 비어 있습니다.");
            return;
        }

        StartCoroutine(SpawnRoutine(arr));
    }

    private IEnumerator SpawnRoutine(SpawnData[] datas)
    {
        foreach (SpawnData data in datas)
        {
            // 각 항목의 delay(초)만큼 대기한 뒤 스폰
            yield return new WaitForSeconds(data.delay);

            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.LogWarning("LoadMain: SpawnPoint가 없습니다.");
                continue;
            }

            if (data.point < 0 || data.point >= spawnPoints.Length || spawnPoints[data.point] == null)
            {
                Debug.LogWarning($"LoadMain: 잘못된 spawn point index={data.point}");
                continue;
            }

            GameObject prefab = GetEnemyPrefab(data.enemyType);
            if (prefab == null)
            {
                Debug.LogWarning($"LoadMain: EnemyType({data.enemyType})에 해당하는 프리팹이 없습니다.");
                continue;
            }

            Transform point = spawnPoints[data.point];
            GameObject enemyGo = Instantiate(prefab, point.position, Quaternion.identity);

            // 아래 방향으로 이동 시작
            Enemy enemy = enemyGo.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.StartMove(Vector2.down);
            }
        }
    }

    private GameObject GetEnemyPrefab(SpawnData.EnemyType enemyType)
    {
        switch (enemyType)
        {
            case SpawnData.EnemyType.A: return enemyAPrefab;
            case SpawnData.EnemyType.B: return enemyBPrefab;
            case SpawnData.EnemyType.C: return enemyCPrefab;
            default: return null;
        }
    }

    private void AutoCollectSpawnPointsIfEmpty()
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
}
