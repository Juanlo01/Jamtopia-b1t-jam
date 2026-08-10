using System.Collections;
using UnityEngine;

public class UI_EvidenceList : MonoBehaviour
{
    [Header("Target UI Settings")]
    [Tooltip("The RectTransform to move. Leave blank to move THIS object.")]
    [SerializeField] private RectTransform uiElementToMove;

    [Header("Movement Settings")]
    [SerializeField] private float moveDistance = 300f; // Pixels to move on X axis
    [SerializeField] private float duration = 0.3f;     // Animation time in seconds

    private bool isMoving = false;
    private bool isOpen = false; // Tracks whether the panel is currently moved right

    private void Awake()
    {
        // Default to this object's RectTransform if unassigned
        if (uiElementToMove == null)
        {
            uiElementToMove = GetComponent<RectTransform>();
        }
    }

    /// <summary>
    /// Attach this method to your UI Button's OnClick() event!
    /// </summary>
    public void ToggleMove()
    {
        if (isMoving) return;

        // If open, move -300px back left. If closed, move +300px right.
        float offset = isOpen ? -moveDistance : moveDistance;
        
        StartCoroutine(AnimateMove(offset));
    }

    private IEnumerator AnimateMove(float xOffset)
    {
        isMoving = true;

        Vector2 startPos = uiElementToMove.anchoredPosition;
        Vector2 targetPos = startPos + new Vector2(xOffset, 0f);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Smoothly interpolate position (Ease Out)
            t = Mathf.SmoothStep(0f, 1f, t);

            uiElementToMove.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        uiElementToMove.anchoredPosition = targetPos;
        isOpen = !isOpen; // Flip state after movement finishes
        isMoving = false;
    }
}