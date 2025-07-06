using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    [SerializeField] int[] checkpoints;
    [SerializeField] FollowSpline followSpline;
    [SerializeField] GuideUI guideUI;
    [SerializeField] float acceleration = 5f;
    public float ProgressRatio { get; private set; } = 0f;
    private int currentCheckpointIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnCheckpoint(GameObject[] checkpointGuide, int newPages)
    {
        followSpline.OnReachedCheckpoint(acceleration);

        guideUI.SetPage(checkpointGuide, newPages);
        guideUI.ShowGuideUI();
    }

    public void OffCheckpoint()
    {
        followSpline.OffReachedCheckpoint(acceleration);

        guideUI.HideGuideUI();
        currentCheckpointIndex++;
    }
}
