using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] GameObject[] checkpointGuide;
    [SerializeField] int newPages;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Spaceship"))
        {
            ProgressManager.Instance?.OnCheckpoint(checkpointGuide, newPages);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Spaceship"))
        {
            ProgressManager.Instance?.OffCheckpoint();

            GameObject.Destroy(gameObject);
        }
    }
}
