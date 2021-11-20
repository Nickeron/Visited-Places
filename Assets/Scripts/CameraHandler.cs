using UnityEngine;

[RequireComponent(typeof(Camera), typeof(Skybox))]
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
        RotateGameObject();
    }

    /// <summary>
    /// Creates a simple mesh, of 1 triangle, to be used as a collision mesh from the given MeshFilter.   
    /// </summary>
    /// <param name="childMeshCollider">The MeshFilter component to fill with the new Mesh</param>
    /// TODO: Remove MeshRenderer from children, and leave only the collider to increase performance
    void CreateColliderMeshAt(MeshFilter childMeshCollider)
    {
        Mesh boundaryMesh = new();

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

    /// <summary>
    /// Calculates a vector with cosine (as x) and sine (as y) (z = 0) for the given angle  
    /// </summary>
    /// <param name="angle">The angle in Degrees converted to RAD by the method</param>
    /// <returns>new Vector3(x: cosine(angle), y: sine(angle), z: 0)</returns>
    private Vector3 GetVectorFromAngleOnY(float angle)
    {
        // Angle = 0 -> 360
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    /// <summary>
    /// Rotates the gameobject it resides, clockwise.
    /// </summary>
    void RotateGameObject()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// Sets the output texture for the camera component.
    /// </summary>
    /// <param name="cardRawImage">The RenderTexture element to output the feed of the camera to.</param>
    public void SetRenderTexture(Texture cardRawImage)
    {
        if (cardRawImage == null) return;

        GetComponent<Camera>().targetTexture = cardRawImage as RenderTexture;
    }

    /// <summary>
    /// Sets the skybox component to be used as a background from the camera component.
    /// </summary>
    /// <param name="customSkybox">The material to be used as a skybox by the camera.</param>
    public void SetCameraSky(Material customSkybox)
    {
        if (customSkybox == null) return;

        GetComponent<Skybox>().material = customSkybox;
    }

    /// <summary>
    /// Calculates the camera's view frustum and returns the limits of the viewing frame  
    /// </summary>
    /// <returns>an array of Plane</returns>
    public Plane[] GetFrustumPlanes()
    {
        return GeometryUtility.CalculateFrustumPlanes(GetComponent<Camera>());
    }
}
