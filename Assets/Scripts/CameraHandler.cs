using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [Range(5f, 10f)]
    public float rotationSpeed = 7f;

    public float viewDistance = 100f;
    public float angle = 0;
    public float fOV = 50f;

    [SerializeField]
    [Tooltip("Used to activate spawning points")]
    MeshFilter rightMostCollider = null;

    [SerializeField]
    [Tooltip("Used to de-activate spawning points")]
    MeshFilter lefftMostCollider = null;

    private void Start()
    {
        // Create a Mesh ahead of the camera's Field of View.
        // So as to know WHERE the camera will be looking at.
        // Camera Frustum Planes, not useful for this.
        CreateColliderMeshAt(rightMostCollider);

        // Create a Mesh behind camera's Field of View.
        // So as to know WHEN the camera is not looking there anymore.
        CreateColliderMeshAt(lefftMostCollider);
    }

    void FixedUpdate()
    {
        RotateCamera();
    }

    void CreateColliderMeshAt(MeshFilter childMeshCollider)
    {
        Mesh boundaryMesh = new Mesh();
        childMeshCollider.sharedMesh = boundaryMesh;
        childMeshCollider.GetComponent<MeshCollider>().sharedMesh = boundaryMesh;

        Vector3[] vertices = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero + GetVectorFromAngleOnY(angle) * viewDistance,
            Vector3.zero + GetVectorFromAngleOnY(fOV) * viewDistance
        };

        int[] triangles = new int[] { 0, 1, 2 };

        boundaryMesh.vertices = vertices;
        boundaryMesh.triangles = triangles;
        boundaryMesh.uv = new Vector2[vertices.Length];
    }

    private Vector3 GetVectorFromAngleOnY(float angle)
    {
        // Angle = 0 -> 360
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    void RotateCamera()
    {
        transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0));
    }
}
