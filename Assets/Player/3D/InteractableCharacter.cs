using UnityEngine;
using Yarn.Unity;

public class InteractableCharacter : MonoBehaviour, IInteractable3D{
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private string dialogueNode = "Start";

    [SerializeField] private CharacterController3D characterController;

    private PlayerController3D interactingPlayer;
    private System.Action onInteractionEnd;

    // interact(), for any class interfacing IInteractable3D
    public void Interact(PlayerController3D interactingPlayer, System.Action onInteractionEnd){
        this.interactingPlayer = interactingPlayer;
        this.onInteractionEnd = onInteractionEnd;

        interactingPlayer.SetMovementFrozen(true);
        characterController.IdleFace(interactingPlayer.transform.position);

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
