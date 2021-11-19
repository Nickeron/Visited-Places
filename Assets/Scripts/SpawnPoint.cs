using UnityEngine;
using UnityEngine.Pool;

public class SpawnPoint : MonoBehaviour
{
    private ObjectPool<GameObject> decorativesPool;
    private GameObject _decorative;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger");
        if (other == null) return;

        if (other.gameObject.CompareTag("SpawnActivation"))
        {
            RequestDecorativeObject();
            return;
        }

        if (other.gameObject.CompareTag("SpawnDeactivation")) ReleaseDecorativeObject();
    }

    void RequestDecorativeObject()
    {
        _decorative = decorativesPool.Get();
        _decorative.transform.position = transform.position;
    }

    void ReleaseDecorativeObject()
    {
        if (_decorative != null)
        {
            decorativesPool.Release(_decorative);
        }
    }

    public void SetPositionAndPool(Vector3 position, ObjectPool<GameObject> newPool, Plane[] frustumPlanes)
    {
        transform.localPosition = position;
        decorativesPool = newPool;
        Activate();

        if (IsVisible(frustumPlanes)) RequestDecorativeObject();
    }

    private bool IsVisible(Plane[] frustumPlanes) => GeometryUtility.TestPlanesAABB(frustumPlanes, GetComponent<Collider>().bounds);

    private void Activate()
    {
        GetComponent<Collider>().enabled = true;
    }

    public void ResetProperties()
    {
        GetComponent<Collider>().enabled = false;

        if (_decorative != null) Destroy(_decorative);
    }
}
