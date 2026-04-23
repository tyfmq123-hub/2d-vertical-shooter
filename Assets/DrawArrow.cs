using UnityEngine;

public class DrawArrow
{
    public static void ForDebug2D(Vector3 start, Vector3 dir, float length)
    {
        Vector3 end = start + dir.normalized * length;

        Debug.DrawLine(start, end, Color.red);

        // 화살표 날개
        Vector3 right = Quaternion.Euler(0, 0, 30) * -dir.normalized;
        Vector3 left = Quaternion.Euler(0, 0, -30) * -dir.normalized;

        Debug.DrawLine(end, end + right, Color.red);
        Debug.DrawLine(end, end + left, Color.red);
    }
}
