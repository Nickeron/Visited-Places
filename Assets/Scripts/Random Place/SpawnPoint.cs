using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Used as a component on an empty GameObject, with just a sphere collider (the most efficient)
/// Requests a decorative from an ObjectPool, when it collides with the Activation MeshCollider and releases it back
/// when it collides with the Deactivation MeshCollider.
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class SpawnPoint : MonoBehaviour
{
    private ObjectPool<GameObject> _decorativesPool;
    private GameObject _decorative;

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        if (other.gameObject.CompareTag("SpawnActivation"))
        {
            RequestDecorativeObject();
            return;
        }

        if (other.gameObject.CompareTag("SpawnDeactivation")) ReleaseDecorativeObject();
    }

    /// <summary>
    /// Requests a prefab from the pool and gives it it's position
    /// </summary>
    void RequestDecorativeObject()
    {
        _decorative = _decorativesPool.Get();
        _decorative.transform.position = transform.position;
    }

    /// <summary>
    /// Release the prefab back to the pool
    /// </summary>
    void ReleaseDecorativeObject()
    {
        if (_decorative != null)
        {
            _decorativesPool.Release(_decorative);
        }
    }

    /// <summary>
    /// Sets the position of the spawner a click below the provided vertex as if it touches the ground.
    /// If the spawner is visible to the camera it requests a decorative object from the provided pool, to display at it's position.
    /// </summary>
    /// <param name="position">Vertex Position</param>
    /// <param name="newPool">ObjectPool to request decoratives from</param>
    /// <param name="frustumPlanes">To calculate if this spawner is visible by the camera</param>
    public void SetPositionAndPool(Vector3 position, ObjectPool<GameObject> newPool, Plane[] frustumPlanes)
    {
        transform.localPosition = position + Vector3.down * 0.1f;
        _decorativesPool = newPool;
        Activate();

        if (IsVisible(frustumPlanes)) RequestDecorativeObject();
    }

    /// <summary>
    /// Calculate if this spawner is visible to the camera.
    /// </summary>
    /// <param name="frustumPlanes">To use at the AABB calculation</param>
    /// <returns>True if the spawner is visible to the camera.</returns>
    private bool IsVisible(Plane[] frustumPlanes) => GeometryUtility.TestPlanesAABB(frustumPlanes, GetComponent<Collider>().bounds);

    private void Activate()
    {
        GetComponent<Collider>().enabled = true;
    }

    /// <summary>
    /// Deactivates spawner's collider and destroys its prefab.
    /// </summary>
    public void ResetProperties()
    {
        GetComponent<Collider>().enabled = false;

        if (_decorative != null) Destroy(_decorative);
    }
}
