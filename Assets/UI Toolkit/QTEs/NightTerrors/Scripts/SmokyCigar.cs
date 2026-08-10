using UnityEngine;
using UnityEngine.UIElements;

public class SmokyDragController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    
    [Header("Defeat Settings")]
    [SerializeField] private float dragDistanceThreshold = 300f; // Pixels dragged to defeat

    private VisualElement smokyOverlay; // Full parent overlay
    private VisualElement smokyElement; // Your visual element named "Smoky"
    
    private bool isDragging = false;
    private Vector2 startPointerPos;

    private void OnEnable()
    {
        VisualElement root = uiDocument.rootVisualElement;

        // 1. Query the exact UXML element named "Smoky"
        smokyElement = root.Q<VisualElement>("Smoky");
        smokyOverlay = root.Q<VisualElement>("SmokyOverlay"); // Or whatever parent container wraps all smoke

        if (smokyElement != null)
        {
            // Enable click detection on Smoky
            smokyElement.pickingMode = PickingMode.Position;

            // Register drag events
            smokyElement.RegisterCallback<PointerDownEvent>(OnPointerDown);
            smokyElement.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            smokyElement.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }
        else
        {
            Debug.LogError("Could not find a VisualElement named 'Smoky' in the UXML hierarchy!");
        }
    }

    private void OnDisable()
    {
        if (smokyElement != null)
        {
            smokyElement.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            smokyElement.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            smokyElement.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        }
    }

    private void OnPointerDown(PointerDownEvent evt)
    {
        isDragging = true;
        
        // Capture screen-space pointer position at click start
        startPointerPos = evt.position;
        
        // Retain pointer capture so dragging doesn't break if cursor leaves Smoky's bounds
        smokyElement.CapturePointer(evt.pointerId);
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
        if (!isDragging) return;

        // Delta distance from initial click point
        Vector2 delta = (Vector2)evt.position - startPointerPos;

        // Translate moves Smoky relative to where he was originally rendered in UI
        smokyElement.style.translate = new StyleTranslate(
            new Translate(delta.x, delta.y, 0)
        );

        // Check if dragged beyond threshold
        if (delta.magnitude >= dragDistanceThreshold)
        {
            DefeatSmoky(evt.pointerId);
        }
    }

    private void OnPointerUp(PointerUpEvent evt)
    {
        if (!isDragging) return;

        isDragging = false;
        smokyElement.ReleasePointer(evt.pointerId);

        // Snap back to original position if released too early
        smokyElement.style.translate = new StyleTranslate(new Translate(0, 0, 0));
    }

    private void DefeatSmoky(int pointerId)
    {
        isDragging = false;

        if (smokyElement.HasPointerCapture(pointerId))
        {
            smokyElement.ReleasePointer(pointerId);
        }

        // Hide Smoky completely
        if (smokyOverlay != null)
        {
            smokyOverlay.style.display = DisplayStyle.None;
        }
        else
        {
            smokyElement.style.display = DisplayStyle.None;
        }

        Debug.Log("Smoky was successfully dragged away and defeated!");
    }
}