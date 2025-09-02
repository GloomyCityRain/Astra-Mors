using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    private Mesh mesh;

    // Direction the cone should face (set from enemy movement)
    [HideInInspector] public Vector2 facingDir = Vector2.up;

    public LayerMask Guards, Player;

    [Header("Detection Settiongs")]
    public float detectionTime = 3f;
    public bool playerSpotted = false; 
    private float currentDetection = 0f;
    public bool playerInside = false;
    private MeshRenderer meshRenderer;
    private Color coneColour;
    private PolygonCollider2D polygonCollider;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        meshRenderer = GetComponent<MeshRenderer>();
        coneColour = meshRenderer.material.color;
        coneColour.a = 0f;
        meshRenderer.material.color = coneColour;

        polygonCollider = GetComponent<PolygonCollider2D>();
        polygonCollider.isTrigger = true; 
    }

    void Update()
    {
        float fov = 90f;
        Vector3 origin = transform.position;
        origin.z = 0f;

        int rayCount = 50;
        float viewDistance = 20f;

        float angleIncrease = fov / rayCount;

        // Calculate the base angle from facingDir
        float baseAngle = Mathf.Atan2(facingDir.y, facingDir.x) * Mathf.Rad2Deg;
        float angle = baseAngle + fov / 2f;

        Vector3[] vertices = new Vector3[rayCount + 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = Vector3.zero; // center in local space

        int vertexIndex = 1;
        int triangleIndex = 0;

        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 dir = GetVectorFromAngle(angle);

            RaycastHit2D hit = Physics2D.Raycast(origin, dir, viewDistance, ~Guards & ~Player);
            Vector3 vertex;
            if (hit.collider == null)
            {
                vertex = origin + dir * viewDistance;
            }
            else
            {
                vertex = hit.point;
            }

            // Convert world position to local space for mesh
            vertices[vertexIndex] = transform.InverseTransformPoint(vertex);

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;
                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= angleIncrease;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        //Detection progress bar for the player

        Vector2[] colliderPoints = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            colliderPoints[i] = vertices[i];
        }
        polygonCollider.SetPath(0, colliderPoints);

        if (playerInside)
        {
            currentDetection += Time.deltaTime;
        }
        else
        {
            currentDetection -= Time.deltaTime;
        }
        currentDetection = Mathf.Clamp(currentDetection, 0f, detectionTime);
        float alpha = currentDetection / detectionTime;
        coneColour.a = alpha;
        meshRenderer.material.color = coneColour;

        if (currentDetection >= detectionTime)
        {
            Debug.Log("Player Found");
        }
        else if (currentDetection >= detectionTime / 3)
        {
            playerSpotted = true;
        }

        Debug.Log(playerInside);
    }

    // Converts angle in degrees to a normalized direction
    public static Vector3 GetVectorFromAngle(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInside = false; 
        }
    }
}
