using UnityEngine;

public class CubeBoundingBox : MonoBehaviour
{
    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 24; // 12 edges (2 points each)
        lineRenderer.loop = false;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material.color = Color.black;
    }

    private void Update()
    {
        DrawEdges(); // Update the edges every frame
    }

    private void DrawEdges()
    {
        Bounds bounds = GetComponent<MeshRenderer>().bounds;

        // Define the 8 corners of the bounding box
        Vector3[] corners = new Vector3[8];
        corners[0] = bounds.min; // (xMin, yMin, zMin)
        corners[1] = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z); // (xMin, yMin, zMax)
        corners[2] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z); // (xMin, yMax, zMin)
        corners[3] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z); // (xMax, yMin, zMin)
        corners[4] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z); // (xMax, yMax, zMin)
        corners[5] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z); // (xMin, yMax, zMax)
        corners[6] = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z); // (xMax, yMin, zMax)
        corners[7] = bounds.max; // (xMax, yMax, zMax)

        // Define edges as pairs of corners
        Vector3[] edges = {
            corners[0], corners[1], // Z axis (min.x, min.y)
            corners[0], corners[2], // Y axis (min.x, min.z)
            corners[0], corners[3], // X axis (min.y, min.z)

            corners[1], corners[5], // Y axis (min.x, max.z)
            corners[1], corners[6], // X axis (min.y, max.z)

            corners[2], corners[5], // Z axis (min.x, max.y)
            corners[2], corners[4], // X axis (max.y, min.z)

            corners[3], corners[6], // Z axis (max.x, min.y)
            corners[3], corners[4], // Y axis (max.x, min.z)

            corners[4], corners[7], // Z axis (max.x, max.y)
            corners[5], corners[7], // X axis (max.y, max.z)
            corners[6], corners[7], // Y axis (max.x, max.z)
        };

        lineRenderer.positionCount = edges.Length;
        lineRenderer.SetPositions(edges);
    }
}
