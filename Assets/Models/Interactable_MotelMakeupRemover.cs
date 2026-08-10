using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;
using System.Collections;

public class Interactable_MotelMakeupRemover : MonoBehaviour, IInteractable3D{
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private string dialogueNode = "motelInteractMakeupWipes";
    [SerializeField] private InputAction press, screenPos;
    [SerializeField] private MeshRenderer brother;
    [SerializeField] private LayerMask interactableLayer;

    private PlayerController3D interactingPlayer;
    private System.Action onInteractionEnd;

    private Vector3 currentScreenPos;

    [SerializeField] public new Camera camera;
    bool isDragging;
    bool motelHairbrushCollected;

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
            //camera = Camera.main;
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

    // interact(), for any class interfacing IInteractable3D
    public void Interact(PlayerController3D interactingPlayer, System.Action onInteractionEnd){
        this.interactingPlayer = interactingPlayer;
        this.onInteractionEnd = onInteractionEnd;

        interactingPlayer.SetMovementFrozen(true);

        if(dialogueRunner != null && !string.IsNullOrEmpty(dialogueNode)){
            dialogueRunner.onDialogueComplete.AddListener(HandleDialogueComplete);
            dialogueRunner.StartDialogue(dialogueNode).Forget();
           
            Debug.Log("Dialogue node selected is " + dialogueNode);
        }
        else{
            EndInteraction();
        }
    }

    // callback for when listener to dialogue runner receives an end signal
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
