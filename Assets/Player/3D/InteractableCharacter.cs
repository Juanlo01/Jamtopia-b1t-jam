using UnityEngine;
using Yarn.Unity;

public class InteractableCharacter : MonoBehaviour, IInteractable3D{
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private string dialogueNode = "Start";

    private PlayerController3D interactingPlayer;
    private System.Action onInteractionEnd;

    public void Interact(PlayerController3D interactingPlayer, System.Action onInteractionEnd){
        Debug.Log($"[InteractableCharacter] {name} interacted with by {interactingPlayer.name}");
        this.interactingPlayer = interactingPlayer;
        this.onInteractionEnd = onInteractionEnd;

        interactingPlayer.SetMovementFrozen(true);
        Debug.Log($"[InteractableCharacter] {name} froze {interactingPlayer.name}'s movement");

        if(dialogueRunner != null && !string.IsNullOrEmpty(dialogueNode)){
            Debug.Log($"[InteractableCharacter] {name} starting dialogue node \"{dialogueNode}\"");
            dialogueRunner.onDialogueComplete.AddListener(HandleDialogueComplete);
            dialogueRunner.StartDialogue(dialogueNode).Forget();
        }
        else{
            Debug.Log($"[InteractableCharacter] {name} has no dialogue configured, ending interaction immediately");
            EndInteraction();
        }
    }

    private void HandleDialogueComplete(){
        Debug.Log($"[InteractableCharacter] {name} dialogue complete");
        dialogueRunner.onDialogueComplete.RemoveListener(HandleDialogueComplete);
        EndInteraction();
    }

    private void EndInteraction(){
        Debug.Log($"[InteractableCharacter] {name} ending interaction, unfreezing {interactingPlayer.name}'s movement");
        interactingPlayer.SetMovementFrozen(false);
        onInteractionEnd?.Invoke();
        interactingPlayer = null;
        onInteractionEnd = null;
    }

    public void OnTouchingPlayer(){
        Debug.Log($"[InteractableCharacter] {name} is now touching player");
    }
    public void OnNotTouchingPlayer(){
        Debug.Log($"[InteractableCharacter] {name} is no longer touching player");
    }
}
