using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] GameObject[] checkpointGuide;
    [SerializeField] int newPages;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Spaceship"))
        {
            Debug.Log("Spaceship is entering checkpoint");
            ProgressManager.Instance?.OnCheckpoint(newPages);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Spaceship"))
        {
            Debug.Log("Spaceship is exiting checkpoint");
            ProgressManager.Instance?.OffCheckpoint();

            GameObject.Destroy(gameObject);
        }
    }
}
