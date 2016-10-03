using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class DrawGizmosOnCollider : MonoBehaviour
{

    // Collider object.
    private PolygonCollider2D polyCollider;

    // Points on the border of the collider.
    private Vector2[] borderPoints;

    // The transform position of the collider.
    private Vector3 transformPos;

    // Use this for initialization
#if UNITY_EDITOR
    void Start()
    {
        polyCollider = GetComponent<PolygonCollider2D>();
        borderPoints = polyCollider.points;
        transformPos = polyCollider.transform.position;
    }
#endif
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (EditorApplication.isPlaying)
            return;

        if (borderPoints == null || borderPoints.Length == 0)
            return;

        Gizmos.color = Color.green;
        // Connect the points to draw the complete shape of the collider.
        for (int i = 0; i < borderPoints.Length - 1; i++)
        {
            Gizmos.DrawLine(new Vector3(borderPoints[i].x + transformPos.x, borderPoints[i].y + transformPos.y),
                new Vector3(borderPoints[i + 1].x + transformPos.x, borderPoints[i + 1].y + transformPos.y));
        }

        // Draw last line.
        if (borderPoints.Length > 1)
        {
            // Draw line from last point to first point.
            Gizmos.DrawLine(new Vector3(borderPoints[borderPoints.Length - 1].x + transformPos.x, borderPoints[borderPoints.Length - 1].y + transformPos.y),
                new Vector3(borderPoints[0].x + transformPos.x, borderPoints[0].y + transformPos.y));
        }
    }
#endif
}
