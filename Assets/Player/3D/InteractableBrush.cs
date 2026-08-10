using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;
using System.Collections;

public class InteractableBrush : MonoBehaviour, IInteractable3D {
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private string dialogueNode = "Start";
    [SerializeField] private InputAction press, screenPos;
    [SerializeField] private MeshRenderer brother;
    [SerializeField] private LayerMask interactableLayer;

    private PlayerController3D interactingPlayer;
    private System.Action onInteractionEnd;

    private Vector3 currentScreenPos;

    [SerializeField] public new Camera camera;
    bool isDragging;

    // --- NEW: Padding to keep object mesh bounds cleanly inside camera edges ---
    [Header("Camera Clamping")]
    [Tooltip("Padding from screen edges in viewport percent (0.05 = 5% margin from edge)")]
    [SerializeField] private Vector2 edgePadding = new Vector2(0.05f, 0.05f);

    private void Update()
    {
        if (brother.enabled != false)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
        }   
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
        if (brother != null)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            if (camera == null) camera = Camera.main; // Fallback if camera is unassigned
            screenPos.Enable();
            press.Enable();
            screenPos.performed += context => { currentScreenPos = context.ReadValue<Vector2>();};
            press.performed += _ => { if(IsClickedOn) StartCoroutine(Drag()); };
            press.canceled += _ => {isDragging = false; };
        }
        else if (brother == null)
        {
            throw new Exception(gameObject.name +" has no brother");
        }
    }

    private IEnumerator Drag()
    {
        isDragging = true;
        
        // Lock the initial Y height of the object
        float fixedY = transform.position.y; 
        
        Vector3 initialWorldPos = WorldPos;
        Vector3 offset = transform.position - initialWorldPos;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.useGravity = false; 

        while (isDragging)
        {
            Vector3 targetPos = WorldPos + offset;

            // ------------------------------------------------------------------
            // CAMERA CLAMPING LOGIC
            // ------------------------------------------------------------------
            // 1. Convert the world position to Camera Viewport space (X/Y range: 0.0 to 1.0)
            Vector3 viewportPos = camera.WorldToViewportPoint(targetPos);

            // 2. Clamp X and Y within camera view boundaries (with padding)
            viewportPos.x = Mathf.Clamp(viewportPos.x, 0f + edgePadding.x, 1f - edgePadding.x);
            viewportPos.y = Mathf.Clamp(viewportPos.y, 0f + edgePadding.y, 1f - edgePadding.y);

            // 3. Convert clamped viewport position back to world space
            Vector3 clampedWorldPos = camera.ViewportToWorldPoint(viewportPos);

            // 4. Apply clamped X and Z positions while keeping fixed Y height
            transform.position = new Vector3(clampedWorldPos.x, fixedY, clampedWorldPos.z);
            // ------------------------------------------------------------------
            
            yield return null;
        }

        if (rb != null) rb.useGravity = true;
    }

    void StopDragging()
    { 
        isDragging = false;
    }

    public void Interact(PlayerController3D interactingPlayer, System.Action onInteractionEnd){
        this.interactingPlayer = interactingPlayer;
        this.onInteractionEnd = onInteractionEnd;

        interactingPlayer.SetMovementFrozen(true);

        if(dialogueRunner != null && !string.IsNullOrEmpty(dialogueNode)){
            dialogueRunner.onDialogueComplete.AddListener(HandleDialogueComplete);
            dialogueRunner.StartDialogue(dialogueNode).Forget();
        }
        else{
            EndInteraction();
        }
    }

    private void HandleDialogueComplete(){
        dialogueRunner.onDialogueComplete.RemoveListener(HandleDialogueComplete);
        EndInteraction();
    }
    
    private void EndInteraction(){
        interactingPlayer.SetMovementFrozen(false);
        onInteractionEnd?.Invoke();
        interactingPlayer = null;
        onInteractionEnd = null;
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