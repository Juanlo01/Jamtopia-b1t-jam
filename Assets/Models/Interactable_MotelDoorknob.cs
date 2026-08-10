using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

public class Interactable_MotelInteractFrontDoor : MonoBehaviour, IInteractable3D
{
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private string dialogueNode = "motelInteract";
    [SerializeField] private InputAction press, screenPos;
    [SerializeField] private MeshRenderer brother;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Camera camera2;

    private PlayerController3D interactingPlayer;
    private System.Action onInteractionEnd;

    private Vector3 currentScreenPos;

    [SerializeField] public new Camera camera;
    private bool isDragging;
    private bool motelHairbrushCollected;

    private void Update()
    {
        if (brother != null && brother.enabled)
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
            if (Physics.Raycast(ray, out hit, 100f, interactableLayer))
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
            screenPos.Enable();
            press.Enable();

            screenPos.performed += context => { currentScreenPos = context.ReadValue<Vector2>(); };
            
            press.performed += _ => 
            { 
                if (IsClickedOn) 
                {
                    // 1. Begin dragging
                    StartCoroutine(Drag()); 

                    // 2. Trigger Dialogue if not already running
                    if (dialogueRunner != null && !dialogueRunner.IsDialogueRunning)
                    {
                        PlayerController3D player = FindFirstObjectByType<PlayerController3D>();
                        if (player != null)
                        {
                            Interact(player, () => {
                                Debug.Log("[Door] Dialogue interaction ended.");
                            });
                        }
                    }
                } 
            };

            press.canceled += _ => { isDragging = false; };
        }
        else
        {
            throw new Exception(gameObject.name + " has no brother assigned!");
        }

        motelHairbrushCollected = false;
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

            // Lock Y axis to the initial height
            transform.position = new Vector3(targetPos.x, fixedY, targetPos.z);

            yield return null;
        }

        if (rb != null) rb.useGravity = true;
    }

    public void cameraTransition()
    {
        camera2.enabled = true;
        camera.enabled = false;
    }

    // Called when interacting via IInteractable3D or Click
    public void Interact(PlayerController3D interactingPlayer, System.Action onInteractionEnd)
    {
        this.interactingPlayer = interactingPlayer;
        this.onInteractionEnd = onInteractionEnd;

        if (interactingPlayer != null)
        {
            interactingPlayer.SetMovementFrozen(true);
        }

        if (dialogueRunner != null && !string.IsNullOrEmpty(dialogueNode))
        {
            dialogueRunner.onDialogueComplete.AddListener(HandleDialogueComplete);
            dialogueRunner.StartDialogue(dialogueNode);

            Debug.Log("Dialogue node started: " + dialogueNode);
        }
        else
        {
            EndInteraction();
        }
    }

    // Callback when DialogueRunner finishes reading the node
    private void HandleDialogueComplete()
    {
        dialogueRunner.onDialogueComplete.RemoveListener(HandleDialogueComplete);
        cameraTransition();
        EndInteraction();
    }

    private void EndInteraction()
    {
        if (interactingPlayer != null)
        {
            interactingPlayer.SetMovementFrozen(false);
        }

        onInteractionEnd?.Invoke();
        interactingPlayer = null;
        onInteractionEnd = null;
    }

    public void OnTouchingPlayer()
    {
        // Add trigger logic here if needed
    }

    public void OnNotTouchingPlayer()
    {
        // Add trigger logic here if needed
    }
}