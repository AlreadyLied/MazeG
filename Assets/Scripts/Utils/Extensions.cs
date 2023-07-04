using UnityEngine;
using System.Collections;

public static class TransformEx
{
    public delegate float Interpolate(float t);

    // Smooth, linear move to destination for duration seconds 
    public static IEnumerator SmoothMove(this Transform transform, float duration, Vector3 dest)
    {
        Vector3 start = transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(start, dest, (elapsed / duration));

            yield return null;
        }
        transform.position = dest;
    }

    public static IEnumerator SmoothMove(this Transform transform, float duration, Vector3 dest, Interpolate interpolate)
    {
        Vector3 start = transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(start, dest, interpolate(elapsed / duration));

            yield return null;
        }
        transform.position = dest;
    }

    public static void LerpLookRotation(this Transform transform, Transform target, float interpolation)
    {
        LerpLookRotation(transform, target.position, interpolation);
    }

    public static void LerpLookRotation(this Transform transform, Vector3 pos, float interpolation)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(pos - transform.position), interpolation);
    }

    public static Vector3 PlayerPositionHeightCorrected(this Transform transform)
    {
        Vector3 pos = Player.Position;
        pos.y = transform.position.y;
        return pos;
    }

    public static Vector3 DirectionToPlayerHeightCorrected(this Transform transform)
    {
        Vector3 dir = Player.Position - transform.position; 
        dir.y = 0;
        return dir;
    }

    public static float DistanceToPlayerHeightCorrected(this Transform transform)
    {
        return Vector3.Distance(transform.position, transform.PlayerPositionHeightCorrected());
    }

    public static void LookAtHeightCorrected(this Transform transform, Transform target)
    {
        Vector3 pos = target.position;
        pos.y = transform.position.y;
        transform.LookAt(pos);
    }
}
