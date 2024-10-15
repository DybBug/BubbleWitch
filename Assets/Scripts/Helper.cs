using UnityEngine;

public static class Helper
{
    public static Vector2 CalculateReflection(Vector2 normal, Vector2 incident)
    {
        return (incident - 2 * (Vector2.Dot(incident, normal)) * normal).normalized;
    }

    public static float CalculateAngle(Vector2 dir)
    {
        var forward = new Vector2(0.0f, 1.0f);
        float dot = Vector2.Dot(forward, dir);

        float forwardLength = forward.magnitude;
        float dirLength = dir.magnitude;

        float angleRadians = Mathf.Acos(dot / (forwardLength * dirLength));

        float angleDegrees = angleRadians * Mathf.Rad2Deg;

        float cross = forward.x * dir.y - forward.y * dir.x;
        float sign = cross < 0.0f ? -1 : 1;
        return sign * angleDegrees;
    }
}