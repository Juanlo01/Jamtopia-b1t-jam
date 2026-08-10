using UnityEngine;

// Owns a character's current animation action-state and facing direction, driving a SpriteAnimator.
// Shared between the player (movement-driven) and NPCs (explicitly told to face something).
public class CharacterController3D : MonoBehaviour
{
    [SerializeField] private SpriteAnimator spriteAnimator;

    private const float DownAngle = -90f; // pure -Z, matches SpriteAnimator's "Down" direction

    private float lastHorizontal;
    private float lastVertical;
    private SpriteAnimator.ActionState lastState = SpriteAnimator.ActionState.Idle;
    private float lastAngle = DownAngle;

    // by the time Start runs, the SpriteAnimator on this object has finished its own setup (Awake/OnValidate),
    // so it's safe to give it an initial state here
    private void Start(){
        SetActionState(SpriteAnimator.ActionState.Idle, DownAngle);
    }

    private void OnEnable(){
        if(spriteAnimator != null) spriteAnimator.OnPropertiesChanged += HandleSpriteAnimatorPropertiesChanged;
    }

    private void OnDisable(){
        if(spriteAnimator != null) spriteAnimator.OnPropertiesChanged -= HandleSpriteAnimatorPropertiesChanged;
    }

    // SpriteAnimator's own properties changed (eg. characterSprite swapped in the Inspector) - replay
    // the current action/direction, forced, so it re-resolves against whatever's now assigned
    private void HandleSpriteAnimatorPropertiesChanged(){
        SetActionState(lastState, lastAngle, force: true);
    }

    public float GetDirAngle() { return Mathf.Atan2(lastVertical, lastHorizontal) * Mathf.Rad2Deg; }

    // drives Walk/Idle + facing angle from raw movement input, eg. called every FixedUpdate by a mover
    public void UpdateMovementAnimation(float horizontal, float vertical){
        bool isMoving = horizontal != 0f || vertical != 0f;
        if(isMoving){
            lastHorizontal = horizontal;
            lastVertical = vertical;
        }
        SetActionState(isMoving ? SpriteAnimator.ActionState.Walk : SpriteAnimator.ActionState.Idle, GetDirAngle());
    }

    // sets Idle and faces towards a world position, eg. an NPC facing whoever just interacted with it
    public void IdleFace(Vector3 targetWorldPosition){
        Vector3 toTarget = targetWorldPosition - transform.position;
        float angle = Mathf.Atan2(toTarget.z, toTarget.x) * Mathf.Rad2Deg;
        SetActionState(SpriteAnimator.ActionState.Idle, angle);
    }

    public void SetActionState(SpriteAnimator.ActionState state, float angle, bool force = false){
        lastState = state;
        lastAngle = angle;
        // some CharacterController3D users (eg. "Corpse" in motel.unity/greenroom.unity) are 3D-mesh
        // props reusing this component only for IdleFace's turn-to-face-player behaviour, and have no
        // sprite to drive -- don't crash on Start() for those.
        if(spriteAnimator != null) spriteAnimator.SetActionState(state, angle, force);
    }
}
