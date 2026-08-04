using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;
using System.Collections;

public class InteractableEvidence : MonoBehaviour, IInteractable3D{
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private string dialogueNode = "Start";
    [SerializeField] private InputAction press, screenPos;
    [SerializeField] private MeshRenderer brother;

    private PlayerController3D interactingPlayer;
    private System.Action onInteractionEnd;

    private Vector3 currentScreenPos;

    [SerializeField] public new Camera camera;
    bool isDragging;

    Vector3 lockPos;
    float lockRot = -90.0f;

    private void Update()
    {
        lockPos = new Vector3(transform.position.x, 4.0f, transform.position.z);
        transform.rotation = Quaternion.Euler(lockRot, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        if (transform.position.y >= 4.0f)
        {
            transform.position = lockPos;
            transform.Translate(Vector3.down * Time.deltaTime);
        } 

        if (brother.enabled == false)
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
    private bool isClickedOn
    {
        get
        {
            Ray ray = camera.ScreenPointToRay(currentScreenPos);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
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
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            //camera = Camera.main;
            screenPos.Enable();
            press.Enable();
            screenPos.performed += context => { currentScreenPos = context.ReadValue<Vector2>();};
            press.performed += _ => { if(isClickedOn) StartCoroutine(Drag()); };
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
        Vector3 offset = transform.position - WorldPos;
        // grab
        GetComponent<Rigidbody>().useGravity = false; 
        while (isDragging)
        {
            // dragging
            transform.position = WorldPos + offset;
            yield return null;
            
        }
        // drop
        GetComponent<Rigidbody>().useGravity = true;

    }

    // interact(), for any class interfacing IInteractable3D
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
