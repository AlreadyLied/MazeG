using UnityEngine;
using System.Collections;

public static class TransformEx
{
    // linear
    public static IEnumerator SmoothMove(this Transform transform, float duration, Vector3 dest)
    {
        Vector3 start = transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, dest, (elapsed / duration));

            yield return null;
        }
        transform.position = dest;
    }
}
