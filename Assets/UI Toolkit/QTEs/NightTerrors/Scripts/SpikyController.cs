using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem; // Uses the New Input System

public class SpikyController : MonoBehaviour
{
    [Header("Targeting Settings")]
    [SerializeField] private string targetTag = "Tool";       // Tag to search for in 3D scene
    [SerializeField] private string uiEnemyName = "SpikyHand"; // UI Builder element Name
    [SerializeField] private float moveSpeed = 5f;             // Tracking speed

    [Header("Push Settings")]
    [SerializeField] private float pushDistanceThreshold = 40f; // UI pixel distance to trigger pushing
    [SerializeField] private float pushForce = 5f;              // Strength of the push

    [Header("Slicing Settings")]
    [SerializeField] private int requiredSlices = 4;           // Slices needed to defeat Spiky
    [SerializeField] private float minSliceVelocity = 300f;     // Minimum swipe speed required
    [SerializeField] private float sliceCooldown = 0.15f;      // Cooldown between cuts

    private int sliceCount = 0;
    private float nextSliceAllowedTime = 0f;
    private bool isInterfering = false;
    private Vector2 lastMousePos;

    private VisualElement uiEnemyHandle;
    private Transform targetTransform;
    private Rigidbody targetRigidbody;
    private Camera mainCamera;

    private void OnEnable()
    {
        sliceCount = 0;
        mainCamera = Camera.main;

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        uiEnemyHandle = root.Q<VisualElement>(uiEnemyName);

        if (uiEnemyHandle == null)
        {
            Debug.LogError($"[SpikyController] Could not find VisualElement named '{uiEnemyName}' in UIDocument!");
        }

        FindTarget();
    }

    private void OnDisable()
    {
        ReleaseTool();
    }

    private void FindTarget()
    {
        GameObject targetObject = GameObject.FindGameObjectWithTag(targetTag);
        if (targetObject != null)
        {
            targetTransform = targetObject.transform;
            targetRigidbody = targetObject.GetComponent<Rigidbody>();

            if (targetRigidbody != null)
            {
                targetRigidbody.constraints |= RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            }
        }
        else
        {
            Debug.LogWarning($"[SpikyController] No GameObject found with tag '{targetTag}'!");
        }
    }

    private void Update()
    {
        if (uiEnemyHandle == null || targetTransform == null) return;

        // 1. Detect mouse/finger swipes across Spiky's UI bounds
        DetectUISlice();

        // SAFEGUARD: Skip frame if UI Toolkit layout values are not computed yet (NaN)
        if (float.IsNaN(uiEnemyHandle.layout.x) || float.IsNaN(uiEnemyHandle.layout.y))
        {
            return;
        }

        // 2. Convert 3D World position to 2D Screen coordinates
        Vector3 screenPos = mainCamera.WorldToScreenPoint(targetTransform.position);
        if (screenPos.z < 0) return; // Ignore if behind camera

        Vector2 targetUiPos = new Vector2(screenPos.x, Screen.height - screenPos.y);

        // Convert screen coordinates to local parent space
        VisualElement parent = uiEnemyHandle.parent;
        Vector2 localTargetUiPos = (parent != null) ? parent.WorldToLocal(targetUiPos) : targetUiPos;

        // 3. Smoothly move toward target (Reads layout.position to work with Left or Right alignment)
        Vector2 currentUiPos = uiEnemyHandle.layout.position;
        Vector2 newUiPos = Vector2.Lerp(currentUiPos, localTargetUiPos, Time.deltaTime * moveSpeed);

        // Clear right-alignment style rule so style.left takes full control
        uiEnemyHandle.style.right = StyleKeyword.Null;
        uiEnemyHandle.style.left = newUiPos.x;
        uiEnemyHandle.style.top = newUiPos.y;

        // 4. Push tool when in range
        float distanceToTarget = Vector2.Distance(newUiPos, localTargetUiPos);
        if (distanceToTarget <= pushDistanceThreshold)
        {
            PushTool(currentUiPos, localTargetUiPos);
        }
        else if (isInterfering)
        {
            ReleaseTool();
        }
    }

    // -------------------------------------------------------------------------
    // UI SLICE DETECTION LOGIC (New Input System)
    // -------------------------------------------------------------------------
    private void DetectUISlice()
    {
        if (Pointer.current == null) return;

        Vector2 currentMousePos = Pointer.current.position.ReadValue();

        // Primary button pressed this frame
        if (Pointer.current.press.wasPressedThisFrame)
        {
            lastMousePos = currentMousePos;
            return;
        }

        // Primary button held down and dragging
        if (Pointer.current.press.isPressed)
        {
            float deltaDistance = (currentMousePos - lastMousePos).magnitude;
            float velocity = deltaDistance / Time.deltaTime;

            // Check if pointer is moving fast enough and cooldown expired
            if (velocity >= minSliceVelocity && Time.time >= nextSliceAllowedTime)
            {
                // UI Toolkit screen coordinates (Y inverted: 0 at top)
                Vector2 uiMousePos = new Vector2(currentMousePos.x, Screen.height - currentMousePos.y);

                // Test if cursor is inside Spiky's UI bounding box
                if (uiEnemyHandle.worldBound.Contains(uiMousePos))
                {
                    OnSliced();
                    nextSliceAllowedTime = Time.time + sliceCooldown;
                }
            }
        }

        lastMousePos = currentMousePos;
    }

    private void OnSliced()
    {
        sliceCount++;
        Debug.Log($"Spiky UI sliced! ({sliceCount}/{requiredSlices})");

        // Flash red using correct UI Toolkit StyleColor property
        uiEnemyHandle.style.unityBackgroundImageTintColor = new StyleColor(Color.red);
        
        CancelInvoke(nameof(ResetColor));
        Invoke(nameof(ResetColor), 0.08f);

        if (sliceCount >= requiredSlices)
        {
            DefeatSpiky();
        }
    }

    private void ResetColor()
    {
        if (uiEnemyHandle != null)
        {
            // Revert tint back to default UI Builder styling
            uiEnemyHandle.style.unityBackgroundImageTintColor = StyleKeyword.Null;
        }
    }

    private void DefeatSpiky()
    {
        Debug.Log("Spiky defeated by slicing!");
        ReleaseTool();
        gameObject.SetActive(false);
    }

    private void PushTool(Vector2 spikyPos, Vector2 targetPos)
    {
        targetTransform.SendMessage("StopDragging", SendMessageOptions.DontRequireReceiver);
        if (targetTransform == null) return;

        if (!isInterfering)
        {
            isInterfering = true;
            targetTransform.SendMessage("StopDragging", SendMessageOptions.DontRequireReceiver);
            targetTransform.SendMessage("SetToolLocked", true, SendMessageOptions.DontRequireReceiver);
        }

        Vector2 uiDirection = (targetPos - spikyPos).normalized;
        if (uiDirection == Vector2.zero) return;

        Vector3 screenDirection = new Vector3(uiDirection.x, -uiDirection.y, 0f);
        Vector3 worldPushDirection = mainCamera.transform.TransformDirection(screenDirection);

        worldPushDirection.y = 0f;
        worldPushDirection = worldPushDirection.normalized;

        if (targetRigidbody != null)
        {
            targetRigidbody.angularVelocity = Vector3.zero;
            targetRigidbody.isKinematic = false;
            targetRigidbody.AddForce(worldPushDirection * pushForce, ForceMode.Force);
        }
        else
        {
            Vector3 currentPos = targetTransform.position;
            Vector3 movement = worldPushDirection * (pushForce * 0.01f * Time.deltaTime);
            targetTransform.position = new Vector3(currentPos.x + movement.x, currentPos.y, currentPos.z + movement.z);
        }
    }

    private void ReleaseTool()
    {
        if (targetTransform != null && isInterfering)
        {
            isInterfering = false;
            targetTransform.SendMessage("SetToolLocked", false, SendMessageOptions.DontRequireReceiver);
        }
    }
}