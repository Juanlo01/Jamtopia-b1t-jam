using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Globalization;

public interface IInteractable3D{
    // Implementers must call onInteractionEnd once their interaction has fully finished
    void Interact(PlayerController3D interactingPlayer, System.Action onInteractionEnd);
    void OnTouchingPlayer();
    void OnNotTouchingPlayer();
}
public class PlayerInteraction3D : MonoBehaviour{
    [SerializeField] private PlayerController3D playerController;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private LayerMask objectLayer;
    [SerializeField] private Material dither;
    [SerializeField]
    public Camera mainCamera;
    public Camera evidenceCamera;
    public InputActionReference clickMouse;
    public bool isInteractable;
    public bool isAsleep;
    

    private IInteractable3D currentInteractable;
    private bool isInteracting;
    private Color sleepMode;
    private Color awakeMode;
    


    void Awake()
    {
        evidenceCamera.enabled = false;
        isAsleep = false;
        playerController.isSleepwalking = isAsleep;
        UnityEngine.ColorUtility.TryParseHtmlString("#FFBF00", out awakeMode);
        dither.SetColor("_LightColor", awakeMode);
    }

    // Update is called once per frame
    void Update(){
        if(Keyboard.current.eKey.wasPressedThisFrame){
            Debug.Log($"[PlayerInteraction3D] E pressed. isInteracting={isInteracting}, currentInteractable={currentInteractable}");
            if(!isInteracting && currentInteractable != null){
                Debug.Log($"[PlayerInteraction3D] Starting interaction with {currentInteractable}");
                isInteracting = true;
                currentInteractable.Interact(playerController, OnInteractionEnd);
            }
        }
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if(mainCamera.enabled == true)
            {
                mainCamera.enabled = false;
                evidenceCamera.enabled = true;
            }
            else
            {
                evidenceCamera.enabled = false;
                mainCamera.enabled = true;
            }
        }

        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            if (!isAsleep)
            {
                if (UnityEngine.ColorUtility.TryParseHtmlString("#90D5FF", out sleepMode))
                {
                    dither.SetColor("_LightColor", sleepMode);
                }
                isAsleep = true;
                playerController.isSleepwalking = isAsleep;
            }
            else if (isAsleep){
                if(UnityEngine.ColorUtility.TryParseHtmlString("#FFBF00", out awakeMode)){
                    dither.SetColor("_LightColor", awakeMode);
                }
                isAsleep = false;
                playerController.isSleepwalking = isAsleep;
            }
        }
    }

    private void OnInteractionEnd(){
        Debug.Log("[PlayerInteraction3D] Interaction ended");
        isInteracting = false;
    }

    private void OnTriggerEnter(Collider collision){
        IInteractable3D interactable = collision.GetComponent<IInteractable3D>();
        if(interactable != null){
            currentInteractable = interactable;

            if (collision.gameObject.CompareTag("Evidence"))
            {
                isInteractable = true;
            }

        }


    }

    private void OnTriggerExit(Collider collision){
        IInteractable3D interactable = collision.GetComponent<IInteractable3D>();
        if(interactable != null && interactable == currentInteractable){
            currentInteractable = null;
            isInteractable = false;
        }


    }

    private void OnTriggerEnter2D(Collider2D collision){
        IInteractable3D interactable = collision.GetComponent<IInteractable3D>();
        if(interactable != null){
            currentInteractable = interactable;
            currentInteractable.OnTouchingPlayer();
        }
    }

    private void OnTriggerExit2D(Collider2D collision){
        IInteractable3D interactable = collision.GetComponent<IInteractable3D>();
        if(interactable != null && interactable == currentInteractable){
            currentInteractable.OnNotTouchingPlayer();
            currentInteractable = interactable;
        }
    }

    private void OnEnable()
    {
        clickMouse.action.Enable();
        clickMouse.action.performed += Click; 
    }

    // private void onDisable()
    // {
    //     clickMouse.action.performed -= Click;
    //     clickMouse.action.Disable();
    // }

    private void CollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Evidence"))
            {
                //isInteractable = true;
            }
    }

    private void CollisionExit(Collision collision)
    {
        //isInteractable = false;
    }
    

    private void Click(InputAction.CallbackContext context)
    {
         Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(Physics.Raycast(ray, out RaycastHit hit, 100f, objectLayer) && isInteractable == true)
        {
            Debug.Log("Clicked on " + hit.collider.gameObject.name + "!");
            //currentInteractable.Interact(playerController, OnInteractionEnd);
            //Destroy(hit.collider.gameObject);
            hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}