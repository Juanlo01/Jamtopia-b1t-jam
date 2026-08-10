using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;
using System.Collections;
using UnityEditor;

public class InteractableTape : MonoBehaviour, IInteractable3D {
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private string dialogueNode = "Start";
    [SerializeField] private InputAction press, screenPos;
    [SerializeField] private GameObject brother;
    [SerializeField] private GameObject fingerPrint;
    [SerializeField] private LayerMask interactableLayer;

    private PlayerController3D interactingPlayer;
    private System.Action onInteractionEnd;
    private Vector3 currentScreenPos;

    [SerializeField] public new Camera camera;
    [SerializeField] bool isDragging;
    [SerializeField] bool phaseOne;
    [SerializeField] bool phaseTwo;
    [SerializeField] bool phaseThree;
    [SerializeField] bool phaseFour;
    [SerializeField] private float rotationSpeed = 0.5f;
    [SerializeField] private InputAction screenPosAction;
    [SerializeField] float airBubbles;
    public Vector2 currentMouseDelta;
    [SerializeField] private float requiredHoldMoveTime = 3.0f; // Duration in seconds
    private float moveTimer = 0f;

    // --- CAMERA CLAMPING ---
    [Header("Camera Clamping")]
    [Tooltip("Padding from screen edges in viewport percent (0.05 = 5% margin from edge)")]
    [SerializeField] private Vector2 edgePadding = new Vector2(0.05f, 0.05f);

    private void Update()
    {
        if (brother.activeSelf == false)
        {
            gameObject.SetActive(true);
        }   

        currentMouseDelta = screenPosAction.ReadValue<Vector2>();
    }

    private Vector3 WorldPos
    {
        get
        {
            float z = camera.WorldToScreenPoint(transform.position).z;
            return camera.ScreenToWorldPoint(currentScreenPos + new Vector3(0, 0, z));   
        }
    }

    private bool IsClickedOn
    {
        get
        {
            Ray ray = camera.ScreenPointToRay(currentScreenPos);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 100f, interactableLayer))
            {
                return hit.transform == transform;
            }
            return false;
        }
    }

    public void Awake()
    {
        airBubbles = 0f;
        phaseOne = true;
        gameObject.SetActive(false);
        
        if (camera == null) camera = Camera.main; // Fallback if camera is unassigned

        screenPosAction.Enable();
        screenPos.Enable();
        press.Enable();
        screenPos.performed += context => { currentScreenPos = context.ReadValue<Vector2>();};
        press.performed += _ => { if(IsClickedOn) StartCoroutine(Drag()); };
        press.canceled += _ => {isDragging = false; };
    }

    private IEnumerator Drag()
    {
        isDragging = true;
        
        // Lock the initial Y height of the object
        float fixedY = transform.position.y; 
        
        Vector3 initialWorldPos = WorldPos;
        Vector3 previousScreenPos = currentScreenPos;
        Vector3 offset = transform.position - initialWorldPos;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.useGravity = false; 

        while (isDragging)
        {
            Vector3 targetPos = WorldPos + offset;
            
            if (phaseOne)
            {
                // Lock position within camera boundaries while maintaining Y height
                transform.position = GetClampedPosition(targetPos, fixedY);
            }
            else if (phaseTwo)
            {
                Vector3 deltaScreen = currentScreenPos - previousScreenPos;
                float rotY = -deltaScreen.x * rotationSpeed;
                transform.Rotate(0f, rotY, 0f, Space.Self);
                previousScreenPos = currentScreenPos;

                float yAngle = Mathf.DeltaAngle(0f, transform.eulerAngles.y);

                if (Mathf.Abs(yAngle - (-45f)) < 1.0f || Mathf.Abs(yAngle - (135f)) < 1.0f)
                {
                    phaseTwo = false;
                    phaseThree = true;
                    isDragging = false;
                }
            }
            else if (phaseThree)
            {
                moveTimer += Time.deltaTime;

                if (moveTimer >= requiredHoldMoveTime)
                {
                    phaseThree = false;
                    phaseFour = true;
                }
            }
            else if (phaseFour)
            {
                // Lock position within camera boundaries while maintaining Y height
                transform.position = GetClampedPosition(targetPos, fixedY);
                fingerPrint.transform.SetParent(transform);
            }
            
            yield return null;
        }

        if (rb != null) rb.useGravity = true;
    }

    // Helper method to convert target position into camera-bounded world space
    private Vector3 GetClampedPosition(Vector3 targetWorldPos, float fixedY)
    {
        // 1. Convert to Viewport coordinates (0.0 to 1.0 range)
        Vector3 viewportPos = camera.WorldToViewportPoint(targetWorldPos);

        // 2. Clamp X and Y inside the viewport frame with padding
        viewportPos.x = Mathf.Clamp(viewportPos.x, 0f + edgePadding.x, 1f - edgePadding.x);
        viewportPos.y = Mathf.Clamp(viewportPos.y, 0f + edgePadding.y, 1f - edgePadding.y);

        // 3. Convert back to World Space
        Vector3 clampedWorldPos = camera.ViewportToWorldPoint(viewportPos);

        // 4. Return clamped X/Z with locked Y
        return new Vector3(clampedWorldPos.x, fixedY, clampedWorldPos.z);
    }

    void StopDragging()
    { 
        isDragging = false;
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (phaseOne)
        {
            phaseOne = false;
            phaseTwo = true;
        }
    }

    public void Interact(PlayerController3D interactingPlayer, System.Action onInteractionEnd){
        throw new System.NotImplementedException();
    }

    private void HandleDialogueComplete(){
        throw new System.NotImplementedException();
    }
    
    private void EndInteraction(){
        throw new System.NotImplementedException();
    }

    public void OnTouchingPlayer()
    {
        throw new System.NotImplementedException();
    }

    public void OnNotTouchingPlayer()
    {
        throw new System.NotImplementedException();
    }
}