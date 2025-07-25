using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using LitMotion;
using LitMotion.Extensions;

public class GuideUI : MonoBehaviour
{
    [SerializeField] RectTransform levelPages;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] GameObject scrollView;
    [SerializeField] private List<GameObject> allPages = new List<GameObject>();
    [SerializeField] private List<GameObject> activePages = new List<GameObject>();
    [SerializeField] private List<PageEventComponent> activePageComponents = new List<PageEventComponent>();
    [SerializeField] private float deltaPosition = 1700f;
    private int lastPageIndex = 0;
    [SerializeField] private bool isVisible = false;
    [SerializeField] private bool isPageTurning = false;
    [SerializeField] private int currentPageIndex = 0;


    public bool IsVisible => isVisible;
    public int TotalPages => allPages.Count;
    public int ActivePagesCount => activePages.Count;
    public int CurrentPageIndex => currentPageIndex;

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
        int startIndex = lastPageIndex;
        int endIndex = Mathf.Min(startIndex + newPageCount, allPages.Count);

        DeactivateAllPages();

        for (int i = startIndex; i < endIndex; i++)
        {
            if (i >= allPages.Count) break;

            ActivatePage(allPages[i]);
        }

        lastPageIndex += endIndex;
        currentPageIndex = 0;
        ShowPage(activePages[currentPageIndex]);
    }

    private void ActivatePage(GameObject page)
    {
        if (page == null || activePages.Contains(page)) return;

        page.SetActive(true);

        if (!activePages.Contains(page))
            activePages.Add(page);

        PageEventComponent eventComponent = page.GetComponent<PageEventComponent>();
        eventComponent?.OnPageActivated();

        if (activePages.Count == 1)
        {
            ShowPage(page);
        }
        else
        {
            HidePage(page);
        }
    }

    private void ShowPage(GameObject page)
    {
        if (page == null || !activePages.Contains(page)) return;
        page.SetActive(true);
        PageEventComponent eventComponent = page.GetComponent<PageEventComponent>();
        eventComponent?.OnPageShown();

        Debug.Log($"Showing page: {page.name}");
    }

    private void DeactivatePage(GameObject page)
    {
        if (page == null || !activePages.Contains(page)) return;

        page.SetActive(false);

        PageEventComponent eventComponent = page.GetComponent<PageEventComponent>();
        eventComponent?.OnPageDeactivated();
    }

    private void HidePage(GameObject page)
    {
        if (page == null || !activePages.Contains(page)) return;

        PageEventComponent eventComponent = page.GetComponent<PageEventComponent>();
        eventComponent?.OnPageHidden();
        Debug.Log($"Hiding page: {page.name}");
    }

    private void DeactivateAllPages()
    {
        if (activePages == null || activePages.Count == 0) return;

        foreach (GameObject page in activePages)
        {
            DeactivatePage(page);
            HidePage(page);
        }
        activePages.Clear();
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
            DeactivateAllPages();
            gameObject.SetActive(false);
            isVisible = false;
        })
        .Bind(x => canvasGroup.alpha = x);
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

    public void TurnPage(float direction)
    {
        if (isPageTurning || activePages.Count == 0) return;

        StartCoroutine(TurnPageCoroutine(direction));
    }

    private IEnumerator TurnPageCoroutine(float direction)
    {
        isPageTurning = true;

        try
        {
            if (direction > 0)
            {
                yield return StartCoroutine(NextPageCoroutine());
            }
            else if (direction < 0)
            {
                yield return StartCoroutine(PreviousPageCoroutine());
            }
        }
        finally
        {
            isPageTurning = false;
        }
    }

    private IEnumerator NextPageCoroutine()
    {
        if (currentPageIndex >= activePages.Count - 1) yield break;

        Debug.Log($"+Current Page Index: {currentPageIndex}, Active Pages Count: {activePages.Count}");
        currentPageIndex++;
        ShowPage(activePages[currentPageIndex]);

        bool animationComplete = false;

        LMotion.Create(levelPages.anchoredPosition.x, levelPages.anchoredPosition.x - deltaPosition, 0.3f)
        .WithOnComplete(() =>
        {
            HidePage(activePages[currentPageIndex - 1]);
            animationComplete = true;
        })
        .Bind(x => levelPages.anchoredPosition = new Vector2(x, levelPages.anchoredPosition.y));

        yield return new WaitUntil(() => animationComplete);
    }
    private IEnumerator PreviousPageCoroutine()
    {
        if (currentPageIndex <= 0) yield break;
        Debug.Log($"-Current Page Index: {currentPageIndex}, Active Pages Count: {activePages.Count}");
        ShowPage(activePages[currentPageIndex - 1]);

        bool animationComplete = false;

        LMotion.Create(levelPages.anchoredPosition.x, levelPages.anchoredPosition.x + deltaPosition, 0.3f)
        .WithOnComplete(() =>
        {
            HidePage(activePages[currentPageIndex]);
            currentPageIndex--;
            animationComplete = true;
        })
        .Bind(x => levelPages.anchoredPosition = new Vector2(x, levelPages.anchoredPosition.y));

        yield return new WaitUntil(() => animationComplete);
    }

    public void ActivateStencilObject()
    {
        foreach (var page in activePages)
        {
            if (page != activePages[currentPageIndex])
            {
                page.SetActive(false);
            }
            else
            {
                page.SetActive(true);
            }
        }
        scrollView.GetComponent<Image>().enabled = false;
    }

    public void DeactivateStencilObject()
    {
        scrollView.GetComponent<Image>().enabled = true;
        foreach (var page in activePages)
        {
            page.SetActive(true);
        }
    }
}
