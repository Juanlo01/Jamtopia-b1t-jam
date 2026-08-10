using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;
using System.Collections;
using UnityEditor;


public class InteractableTape : MonoBehaviour, IInteractable3D{
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
            //camera = Camera.main;
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
                // Lock Y axis to the initial height
                transform.position = new Vector3(targetPos.x, fixedY, targetPos.z);
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
                //if (currentMouseDelta.sqrMagnitude > 0.01f)
                //{
                    moveTimer += Time.deltaTime;

                    if (moveTimer >= requiredHoldMoveTime)
                    {
                        phaseThree = false;
                        phaseFour = true;
                    }
                //}
            }
            else if (phaseFour)
            {
                // Lock Y axis to the initial height
                transform.position = new Vector3(targetPos.x, fixedY, targetPos.z);
                fingerPrint.transform.SetParent(transform);
            }
            
            yield return null;
        }

        if (rb != null) rb.useGravity = true;
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

    // interact(), for any class interfacing IInteractable3D
    public void Interact(PlayerController3D interactingPlayer, System.Action onInteractionEnd){
        throw new System.NotImplementedException();
    }

    // callback for when listener to dialogue runner receives an end signal
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
