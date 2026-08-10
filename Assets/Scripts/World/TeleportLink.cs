using UnityEngine;

// Which side of the link a trigger belongs to. Public so TeleportLinkTrigger (attached to each
// BoxCollider's own GameObject, since that's the only place Unity will deliver OnTriggerEnter) can
// tell this link which pair of marker/focus transforms to use.
public enum TeleportSide { A, B }

// A two-way teleport connection between two zones. Entering triggerA sends the player to markerB
// and moves the camera to focusB; entering triggerB does the reverse. On arrival, the player is
// faced in the direction of travel (previousMarker -> nextMarker), reusing CharacterController3D /
// SpriteAnimator's existing angle-based facing instead of re-deriving Left/Right/Up/Down here.
public class TeleportLink : MonoBehaviour
{
    [Header("Side A")]
    [SerializeField] private BoxCollider triggerA;
    [SerializeField] private Transform markerA;
    [SerializeField] private Transform focusA;

    [Header("Side B")]
    [SerializeField] private BoxCollider triggerB;
    [SerializeField] private Transform markerB;
    [SerializeField] private Transform focusB;

    [Header("Camera")]
    [Tooltip("Moved to focusA/focusB's position and rotation on teleport (eg. the main Camera, or its rig root).")]
    [SerializeField] private Transform playerCamera;

    [Header("Settings")]
    [Tooltip("Re-entry on either trigger is ignored for this long after a teleport, so arriving right next to the other side's collider doesn't immediately bounce the player back.")]
    [SerializeField] private float teleportCooldown = 0.5f;

    private float lastTeleportTime = -Mathf.Infinity;

    // Fired with the side the player just arrived at (not the trigger side they entered).
    // Generic on purpose -- sibling scripts (eg. PoliceDepartmentRoomPublisher) hook this to
    // publish scene-specific meaning without TeleportLink itself knowing what a "room" is.
    public event System.Action<TeleportSide> OnTeleported;

    private void Awake(){
        Wire(triggerA, TeleportSide.A);
        Wire(triggerB, TeleportSide.B);
    }

    private void Wire(BoxCollider trigger, TeleportSide side){
        if(trigger == null) return;

        trigger.isTrigger = true;

        TeleportLinkTrigger relay = trigger.GetComponent<TeleportLinkTrigger>();
        if(relay == null) relay = trigger.gameObject.AddComponent<TeleportLinkTrigger>();
        relay.Init(this, side);
    }

    // called by TeleportLinkTrigger when its collider is entered
    internal void NotifyTriggerEntered(TeleportSide side, Collider other){
        if(Time.time - lastTeleportTime < teleportCooldown) return;

        PlayerController3D player = other.GetComponentInParent<PlayerController3D>();
        if(player == null) return;

        if(side == TeleportSide.A) Teleport(player, markerA, markerB, focusB, TeleportSide.B);
        else Teleport(player, markerB, markerA, focusA, TeleportSide.A);

        lastTeleportTime = Time.time;
    }

    private void Teleport(PlayerController3D player, Transform previousMarker, Transform nextMarker, Transform focus, TeleportSide arrivedSide){
        if(nextMarker == null) return;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if(rb != null){
            rb.position = nextMarker.position;
            rb.linearVelocity = Vector3.zero;
        }
        else{
            player.transform.position = nextMarker.position;
        }

        if(playerCamera != null && focus != null){
            playerCamera.SetPositionAndRotation(focus.position, focus.rotation);
        }

        FaceDirectionOfTravel(player, previousMarker, nextMarker);

        OnTeleported?.Invoke(arrivedSide);
    }

    private void FaceDirectionOfTravel(PlayerController3D player, Transform previousMarker, Transform nextMarker){
        if(previousMarker == null) return;

        CharacterController3D characterController = player.GetComponent<CharacterController3D>();
        if(characterController == null) return;

        Vector3 travel = nextMarker.position - previousMarker.position;
        float angle = Mathf.Atan2(travel.z, travel.x) * Mathf.Rad2Deg;
        characterController.SetActionState(SpriteAnimator.ActionState.Idle, angle);
    }
}
