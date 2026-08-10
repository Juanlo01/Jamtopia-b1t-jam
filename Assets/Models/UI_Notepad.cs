using System.Collections;
using UnityEngine;

public class UI_Notepad : MonoBehaviour
{
    [Header("Page Settings")]
    [Tooltip("Drag your 5 page GameObjects here in order (Page 1 at index 0, Page 2 at index 1, etc.)")]
    [SerializeField] private GameObject[] pages;
    
    private int currentPageIndex = 0; // 0 = Page 1, 1 = Page 2, etc.

    [Header("Target UI Settings")]
    [Tooltip("The RectTransform to move. Leave blank to move THIS object.")]
    [SerializeField] private RectTransform uiElementToMove;

    [Header("Movement Settings")]
    [SerializeField] private float moveDistance = 50f;
    [SerializeField] private float duration = 0.3f;

    private bool isMoving = false;
    private bool isOpen = false;

    private void Awake()
    {
        if (uiElementToMove == null)
        {
            uiElementToMove = GetComponent<RectTransform>();
        }

        // Initialize display to show only the first page on startup
        UpdatePageVisibility();
    }

    /// <summary>
    /// Attach this to your 'Next' Page Button OnClick()
    /// </summary>
    public void NextPage()
    {
        if (pages == null || pages.Length == 0) return;

        // Prevent incrementing past the last page
        if (currentPageIndex < pages.Length - 1)
        {
            currentPageIndex++;
            UpdatePageVisibility();
        }
    }

    /// <summary>
    /// Attach this to your 'Previous' Page Button OnClick()
    /// </summary>
    public void PreviousPage()
    {
        if (pages == null || pages.Length == 0) return;

        // Prevent decrementing below the first page
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            UpdatePageVisibility();
        }
    }

    private void UpdatePageVisibility()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null)
            {
                // Enable only the page matching the current index, disable the rest
                pages[i].SetActive(i == currentPageIndex);
            }
        }
    }

    public void ToggleMove()
    {
        if (isMoving) return;

        float offset = isOpen ? -moveDistance : moveDistance;
        StartCoroutine(AnimateMove(offset));
    }

    private IEnumerator AnimateMove(float yOffset)
    {
        isMoving = true;

        Vector2 startPos = uiElementToMove.anchoredPosition;
        Vector2 targetPos = startPos + new Vector2(0f, yOffset);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            t = Mathf.SmoothStep(0f, 1f, t);

            uiElementToMove.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        uiElementToMove.anchoredPosition = targetPos;
        isOpen = !isOpen;
        isMoving = false;
    }
}