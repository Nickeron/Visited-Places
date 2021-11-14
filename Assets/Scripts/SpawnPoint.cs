using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
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
        Debug.Log($"Requesting a decorative object from {transform.position}");
    }

    void ReleaseDecorativeObject()
    {
        Debug.Log($"Requesting a release from {transform.position}");
    }
}
