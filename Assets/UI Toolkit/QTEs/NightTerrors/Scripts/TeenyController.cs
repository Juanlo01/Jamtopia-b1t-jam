using UnityEngine;
using UnityEngine.UIElements; // Required for UI Toolkit

public class TeenyController : MonoBehaviour
{
    [Header("Targeting Settings")]
    [SerializeField] private string targetTag = "Tool";       // Tag to search for in 3D scene
    [SerializeField] private string uiEnemyName = "Teeny";     // UI Builder element Name
    [SerializeField] private float moveSpeed = 5f;             // Tracking speed

    [Header("Push Settings")]
    [SerializeField] private float pushDistanceThreshold = 40f; // UI pixel distance to trigger pushing
    [SerializeField] private float pushForce = 5f;             // Strength of the push
    [SerializeField] private float timesClicked = 0f;

    private VisualElement uiEnemyHandle;
    private Transform targetTransform;
    private Rigidbody targetRigidbody;
    
    private Camera mainCamera;

    private void OnEnable()
    {
        // Repeat of start code for when Teeny Respawns
        timesClicked = 0;
        // One-line query & click listener setup
        GetComponent<UIDocument>().rootVisualElement.Q<Button>("Button").clicked += () => 
        {
            Debug.Log("Button clicked!");
            timesClicked += 1;
            if (timesClicked >= 2)
            {
                gameObject.SetActive(false);
            }
        };

        
        mainCamera = Camera.main;
        // 1. Get the UI element from UI Toolkit
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        uiEnemyHandle = root.Q<VisualElement>(uiEnemyName);

        // 2. Find the target object in the 3D scene by tag
        GameObject targetObject = GameObject.FindGameObjectWithTag(targetTag);
        if (targetObject != null)
        {
            targetTransform = targetObject.transform;
            targetRigidbody = targetObject.GetComponent<Rigidbody>();

            // Freeze Y position and ALL Rotations (X, Y, Z) on Rigidbody
            if (targetRigidbody != null)
            {
                targetRigidbody.constraints |= RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            }
        }
        else
        {
            Debug.LogWarning($"No GameObject found with tag '{targetTag}'!");
        }
    }

    private void Start()
    {
        // One-line query & click listener setup
        GetComponent<UIDocument>().rootVisualElement.Q<Button>("Button").clicked += () => 
        {
            Debug.Log("Button clicked!");
            timesClicked += 1;
            if (timesClicked >= 2)
            {
                gameObject.SetActive(false);
            }
        };
    }

    private void Update()
    {
        if (uiEnemyHandle == null || targetTransform == null) return;

        // 3. Convert 3D World position to 2D Screen coordinates
        Vector3 screenPos = mainCamera.WorldToScreenPoint(targetTransform.position);

        // Ignore if object is behind camera
        if (screenPos.z < 0) return;

        // UI Toolkit's Y-axis is inverted relative to Screen space (0 is top-left)
        Vector2 targetUiPos = new Vector2(screenPos.x, Screen.height - screenPos.y);

        // Convert screen coordinates to local container space (prevents flying off-screen)
        VisualElement parent = uiEnemyHandle.parent;
        Vector2 localTargetUiPos = (parent != null) ? parent.WorldToLocal(targetUiPos) : targetUiPos;

        // 4. Smoothly move (Lerp) the UI enemy toward the target position
        Vector2 currentUiPos = new Vector2(uiEnemyHandle.resolvedStyle.left, uiEnemyHandle.resolvedStyle.top);
        Vector2 newUiPos = Vector2.Lerp(currentUiPos, localTargetUiPos, Time.deltaTime * moveSpeed);

        // 5. Update UI element coordinates
        uiEnemyHandle.style.left = newUiPos.x;
        uiEnemyHandle.style.top = newUiPos.y;

        // 6. Check distance & push the tool if Teeny reaches it
        float distanceToTarget = Vector2.Distance(newUiPos, localTargetUiPos);
        if (distanceToTarget <= pushDistanceThreshold)
        {
            PushTool(currentUiPos, localTargetUiPos);
        }
    }

    private void PushTool(Vector2 teenyPos, Vector2 targetPos)
    {
        // -------------------------------------------------------------------------
        // Send Message to whatever tool script is attached to this object
        // -------------------------------------------------------------------------
        targetTransform.SendMessage("StopDragging", SendMessageOptions.DontRequireReceiver);

        // Calculate 2D direction Teeny is moving
        Vector2 uiDirection = (targetPos - teenyPos).normalized;
        if (uiDirection == Vector2.zero) return;

        // Convert UI direction to 3D world space
        Vector3 screenDirection = new Vector3(uiDirection.x, -uiDirection.y, 0f);
        Vector3 worldPushDirection = mainCamera.transform.TransformDirection(screenDirection);

        // LOCK Y AXIS MOVEMENT
        worldPushDirection.y = 0f;
        worldPushDirection = worldPushDirection.normalized;

        if (targetRigidbody != null)
        {
            // Stop any residual angular velocity (spinning)
            targetRigidbody.angularVelocity = Vector3.zero;

            // Apply horizontal force only
            targetRigidbody.isKinematic = false;
            targetRigidbody.AddForce(worldPushDirection * pushForce, ForceMode.Force);
        }
        else
        {
            // Fallback: Move position horizontally without changing Y or rotation
            Vector3 currentPos = targetTransform.position;
            Vector3 movement = worldPushDirection * (pushForce * 0.01f * Time.deltaTime);
            
            targetTransform.position = new Vector3(currentPos.x + movement.x, currentPos.y, currentPos.z + movement.z);
        }
    }
}