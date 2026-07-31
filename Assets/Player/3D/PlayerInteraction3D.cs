using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable3D{
    // Implementers must call onInteractionEnd once their interaction has fully finished
    void Interact(PlayerController3D interactingPlayer, System.Action onInteractionEnd);
    void OnTouchingPlayer();
    void OnNotTouchingPlayer();
}
public class PlayerInteraction3D : MonoBehaviour{
    [SerializeField] private PlayerController3D playerController;

    private IInteractable3D currentInteractable;
    private bool isInteracting;

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
    }

    private void OnInteractionEnd(){
        Debug.Log("[PlayerInteraction3D] Interaction ended");
        isInteracting = false;
    }

    private void OnTriggerEnter(Collider collision){
        IInteractable3D interactable = collision.GetComponent<IInteractable3D>();
        if(interactable != null){
            Debug.Log($"[PlayerInteraction3D] Now touching interactable {interactable}");
            currentInteractable = interactable;
            currentInteractable.OnTouchingPlayer();
        }
    }

    private void OnTriggerExit(Collider collision){
        IInteractable3D interactable = collision.GetComponent<IInteractable3D>();
        if(interactable != null && interactable == currentInteractable){
            Debug.Log($"[PlayerInteraction3D] No longer touching interactable {interactable}");
            currentInteractable.OnNotTouchingPlayer();
            currentInteractable = null;
        }
    }
}