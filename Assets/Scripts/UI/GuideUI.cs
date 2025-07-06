using Unity.XR.CoreUtils;
using UnityEngine;

public class GuideUI : MonoBehaviour
{
    [SerializeField] RectTransform levelPages;

    private GameObject[] pages;
    private int lastPageIndex = 0;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetPage(GameObject[] pages, int newPages)
    {
        //foreach (Transform child in levelPages.transform)
        //{
        //    Destroy(child.gameObject);
        //}

        //foreach (GameObject page in pages)
        //{
        //    GameObject newPage = Instantiate(page.gameObject, levelPages.transform);
        //}

        for (int i = 0; i < levelPages.childCount; i++)
        {
            levelPages.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < newPages; i++)
        {
            levelPages.GetChild(i + lastPageIndex).gameObject.SetActive(true);
        }
        lastPageIndex += newPages;
    }

    public void ShowGuideUI()
    {
        gameObject.SetActive(true);
    }

    public void HideGuideUI()
    {
        gameObject.SetActive(false);
    }
}
