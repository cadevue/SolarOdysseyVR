using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

public class GuideUI : MonoBehaviour
{
    [SerializeField] RectTransform levelPages;
    [SerializeField] CanvasGroup canvasGroup;

    [SerializeField] private List<GameObject> allPages = new List<GameObject>();
    [SerializeField] private List<GameObject> activePages = new List<GameObject>();
    private List<PageEventComponent> activePageComponents = new List<PageEventComponent>();
    private int lastPageIndex = 0;
    private bool isVisible = false;

    public bool IsVisible => isVisible;
    public int TotalPages => allPages.Count;
    public int ActivePagesCount => activePages.Count;
    public int CurrentPageIndex => lastPageIndex;

    private void Awake()
    {
        gameObject.SetActive(false);
        canvasGroup.alpha = 0f;
        CacheAllPages();
    }

    public void CacheAllPages()
    {
        allPages.Clear();

        for (int i = 0; i < levelPages.childCount; i++)
        {
            GameObject page = levelPages.GetChild(i).gameObject;
            allPages.Add(page);
            page.SetActive(false);
        }
    }

    public void SetPage(int newPageCount)
    {
        //foreach (Transform child in levelPages.transform)
        //{
        //    Destroy(child.gameObject);
        //}

        //foreach (GameObject page in pages)
        //{
        //    GameObject newPage = Instantiate(page.gameObject, levelPages.transform);
        //}
        int startIndex = lastPageIndex;
        int endIndex = Mathf.Min(startIndex + newPageCount, allPages.Count);

        HideAllPages();

        for (int i = startIndex; i < endIndex; i++)
        {
            if (i >= allPages.Count) break;

            ActivatePage(allPages[i]);
        }

        lastPageIndex += endIndex;
    }

    private void ActivatePage(GameObject page)
    {
        if (page == null || activePages.Contains(page)) return;

        page.SetActive(true);

        if (!activePages.Contains(page))
            activePages.Add(page);

        PageEventComponent eventComponent = page.GetComponent<PageEventComponent>();
        if (!activePageComponents.Contains(eventComponent))
        {
            activePageComponents.Add(eventComponent);
        }
        eventComponent?.OnPageActivated();
        eventComponent?.OnPageShown();
    }

    private void DeactivatePage(GameObject page)
    {
        if (page == null || !activePages.Contains(page)) return;

        page.SetActive(false);

        PageEventComponent eventComponent = page.GetComponent<PageEventComponent>();
        if (activePageComponents.Contains(eventComponent))
        {
            activePageComponents.Remove(eventComponent);
        }
        eventComponent?.OnPageDeactivated();
        eventComponent?.OnPageHidden();
    }

    private void HideAllPages()
    {
        if (activePages == null || activePages.Count == 0) return;

        foreach (GameObject page in activePages)
        {
            DeactivatePage(page);
        }

        activePages.Clear();
        activePageComponents.Clear();
    }

    public void ShowGuideUI()
    {
        canvasGroup.alpha = 0f;
        gameObject.SetActive(true);
        isVisible = true;
        LMotion.Create(canvasGroup.alpha, 1f, 3f).Bind(x => canvasGroup.alpha = x);
    }

    public void HideGuideUI()
    {
        // Use LMotion to create a smooth transition effect
        LMotion.Create(canvasGroup.alpha, 0f, 3f)
        .WithOnComplete(() =>
        {
            HideAllPages();
            gameObject.SetActive(false);
            isVisible = false;
        })
        .Bind(x => canvasGroup.alpha = x)
        ;
    }

    public void ClearPages()
    {
        HideAllPages();
        lastPageIndex = 0;
    }

    public void ToggleVisibility()
    {
        if (isVisible)
        {
            HideGuideUI();
        }
        else
        {
            ShowGuideUI();
        }
    }
}
