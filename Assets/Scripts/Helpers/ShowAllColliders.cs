using UnityEngine;

public class ShowAllColliders : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        // Alle BoxColliders anzeigen
        BoxCollider2D[] boxColliders = FindObjectsOfType<BoxCollider2D>();
        foreach (BoxCollider2D boxCollider in boxColliders)
        {
            Gizmos.color = Color.green;
            Vector3 boxColliderPosition = boxCollider.transform.TransformPoint(boxCollider.offset);
            Gizmos.matrix = boxCollider.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(boxCollider.offset, boxCollider.size);
        }

        // Alle CircleColliders anzeigen
        CircleCollider2D[] circleColliders = FindObjectsOfType<CircleCollider2D>();
        foreach (CircleCollider2D circleCollider in circleColliders)
        {
            Gizmos.color = Color.red;
            Vector3 circleColliderPosition = circleCollider.transform.TransformPoint(circleCollider.offset);
            Gizmos.DrawWireSphere(circleColliderPosition, circleCollider.radius);
        }

        // Alle PolygonColliders anzeigen
        PolygonCollider2D[] polygonColliders = FindObjectsOfType<PolygonCollider2D>();
        foreach (PolygonCollider2D polygonCollider in polygonColliders)
        {
            Gizmos.color = Color.blue;
            Vector2[] points = polygonCollider.points;
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 startPoint = polygonCollider.transform.TransformPoint(points[i] + polygonCollider.offset);
                Vector2 endPoint = polygonCollider.transform.TransformPoint(points[(i + 1) % points.Length] + polygonCollider.offset);
                Gizmos.DrawLine(startPoint, endPoint);
            }
        }

        // Alle EdgeColliders anzeigen
        EdgeCollider2D[] edgeColliders = FindObjectsOfType<EdgeCollider2D>();
        foreach (EdgeCollider2D edgeCollider in edgeColliders)
        {
            Gizmos.color = Color.yellow;
            Vector2[] points = edgeCollider.points;
            for (int i = 0; i < points.Length - 1; i++)
            {
                Vector2 startPoint = edgeCollider.transform.TransformPoint(points[i] + edgeCollider.offset);
                Vector2 endPoint = edgeCollider.transform.TransformPoint(points[i + 1] + edgeCollider.offset);
                Gizmos.DrawLine(startPoint, endPoint);
            }
        }
    }
}
