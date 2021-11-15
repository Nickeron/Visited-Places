using UnityEngine;
using UnityEngine.Pool;

public class SpawnPoint : MonoBehaviour
{
    // An index used for the spawner to request the same type of decorative every time.
    public ObjectPool<GameObject> decorativesPool;
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
        if(_decorative != null)
        {
            decorativesPool.Release(_decorative);
        }        
    }
}
