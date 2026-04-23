using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DistanceLabel : MonoBehaviour
{
    public enum MeasureMode
    {
        TwoTargets,
        ClosestWithTag,
        SpawnerPaths
    }

    [Header("Mode")]
    public MeasureMode mode = MeasureMode.TwoTargets;

    [Header("TwoTargets")]
    public Transform targetA;
    public Transform targetB;

    [Header("ClosestWithTag")]
    public Transform origin;
    public string targetTag = "Enemy";

    [Header("SpawnerPaths")]
    public EnemySpawner[] enemySpawners;
    public bool showLeftSpawner = true;
    public bool showRightSpawner = true;

    [Header("Display")]
    public Color lineColor = Color.yellow;
    public Color textColor = Color.white;
    [Range(10, 30)] public int fontSize = 14;
    public bool showInGameView = false;

    private Transform cachedClosestTarget;
    private float cachedDistance;

    private void OnDrawGizmos()
    {
        if (mode == MeasureMode.SpawnerPaths)
        {
            DrawSpawnerDistances();
            return;
        }

        if (!TryGetMeasurePoints(out Vector3 from, out Vector3 to, out float distance))
        {
            return;
        }

        cachedDistance = distance;

        Gizmos.color = lineColor;
        Gizmos.DrawLine(from, to);

#if UNITY_EDITOR
        Handles.color = textColor;
        Vector3 mid = (from + to) * 0.5f;
        GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
        style.normal.textColor = textColor;
        style.fontSize = fontSize;
        Handles.Label(mid, $"{cachedDistance:F2} m", style);
#endif
    }

    private void DrawSpawnerDistances()
    {
        if (enemySpawners == null || enemySpawners.Length == 0)
        {
            return;
        }

#if UNITY_EDITOR
        GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
        style.normal.textColor = textColor;
        style.fontSize = fontSize;
#endif

        for (int i = 0; i < enemySpawners.Length; i++)
        {
            EnemySpawner spawner = enemySpawners[i];
            if (spawner == null || spawner.startPoint == null || spawner.endPoint == null)
            {
                continue;
            }

            bool isLeft = spawner.direction == EnemySpawner.Direction.LEFT;
            if (isLeft && !showLeftSpawner)
            {
                continue;
            }

            if (!isLeft && !showRightSpawner)
            {
                continue;
            }

            Vector3 from = spawner.startPoint.position;
            Vector3 to = spawner.endPoint.position;
            float distance = Vector3.Distance(from, to);

            Gizmos.color = lineColor;
            Gizmos.DrawLine(from, to);

#if UNITY_EDITOR
            string sideText = isLeft ? "LEFT" : "RIGHT";
            Vector3 mid = (from + to) * 0.5f;
            Handles.Label(mid, $"{sideText}: {distance:F2} m", style);
#endif
        }
    }

    private void OnGUI()
    {
        if (!showInGameView || !Application.isPlaying)
        {
            return;
        }

        string text = $"Distance: {cachedDistance:F2} m";
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 22;
        style.normal.textColor = textColor;
        GUI.Label(new Rect(20f, 20f, 320f, 40f), text, style);
    }

    private bool TryGetMeasurePoints(out Vector3 from, out Vector3 to, out float distance)
    {
        from = Vector3.zero;
        to = Vector3.zero;
        distance = 0f;

        if (mode == MeasureMode.TwoTargets)
        {
            if (targetA == null || targetB == null)
            {
                return false;
            }

            from = targetA.position;
            to = targetB.position;
            distance = Vector3.Distance(from, to);
            return true;
        }

        Transform start = origin != null ? origin : transform;
        Transform closest = FindClosestWithTag(start.position, targetTag);
        if (closest == null)
        {
            return false;
        }

        cachedClosestTarget = closest;
        from = start.position;
        to = closest.position;
        distance = Vector3.Distance(from, to);
        return true;
    }

    private static Transform FindClosestWithTag(Vector3 originPosition, string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName))
        {
            return null;
        }

        GameObject[] taggedObjects;
        try
        {
            taggedObjects = GameObject.FindGameObjectsWithTag(tagName);
        }
        catch
        {
            return null;
        }

        Transform closest = null;
        float minSqr = float.MaxValue;
        for (int i = 0; i < taggedObjects.Length; i++)
        {
            Transform t = taggedObjects[i].transform;
            float sqr = (t.position - originPosition).sqrMagnitude;
            if (sqr < minSqr)
            {
                minSqr = sqr;
                closest = t;
            }
        }

        return closest;
    }

    public Transform GetCachedClosestTarget()
    {
        return cachedClosestTarget;
    }
}
