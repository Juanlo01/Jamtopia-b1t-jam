using UnityEngine;
using UnityEngine.UIElements;

public class SmokyController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    
    [Header("Smoke Fill Settings")]
    [SerializeField] private float riseSpeedPercent = 20f; // Rise speed in percentage per second

    private VisualElement smokeContainer;
    private VisualElement smokeCloud;
    private VisualElement smokeButton1;
    private bool isRising = false;
    private float currentTopPercent = 100f; // 100% means hidden at bottom, 0% means covering screen

    private void OnEnable()
    {
        VisualElement root = uiDocument.rootVisualElement;

        // 1. Pull the Smoke Cloud element from UXML by its Name attribute
        smokeContainer = root.Q<VisualElement>("Smoke"); 
        smokeCloud = root.Q<VisualElement>("SmokeCloud");
        
        // Alternative: If it's a specific class, use root.Q<VisualElement>(className: "smoke-cloud");

        // 2. Query child buttons inside the Smoke Cloud if you need click events
        smokeButton1 = smokeContainer.Q<Button>("SmokeButton1");
        if (smokeButton1 != null)
        {
            smokeButton1.RegisterCallback<ClickEvent>(OnSmokeButtonClicked);
        }

        // Start rising automatically or call StartSmokeRise() from an event
        StartSmokeRise();
    }

    public void StartSmokeRise()
    {
        currentTopPercent = 100f;
        isRising = true;
    }

    private void Update()
    {
        if (!isRising || smokeContainer == null) return;

        // Move the top property from 100% down to 0%
        if (currentTopPercent > 0f)
        {
            currentTopPercent -= riseSpeedPercent * Time.deltaTime;
            currentTopPercent = Mathf.Max(currentTopPercent, 0f);

            // Apply the new position to the UXML element's inline style
            smokeContainer.style.top = Length.Percent(currentTopPercent);
        }
        else
        {
            isRising = false; // Reached top of screen
        }
    }

    private void OnSmokeButtonClicked(ClickEvent evt)
    {
        // Example: Handle button click inside smoke cloud
        Debug.Log("Clicked a button inside the rising smoke!");
        smokeCloud.style.display = DisplayStyle.None;
        smokeButton1.style.display = DisplayStyle.None;
        smokeContainer.pickingMode = PickingMode.Ignore;
    }
}