using UnityEngine;
using UnityEngine.InputSystem; // Allows use of of Input System API

public class PlayerController3D : MonoBehaviour{

    [Header("Player Component References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] SpriteAnimator spriteAnimator;

    [Header("Player Settings")]
    [SerializeField] float speed;
    [SerializeField] float jumpingPower;

    [Header("Grounding")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheck;

    private float horizontal;
    private float vertical;
    private float lastHorizontal;
    private float lastVertical;
    private bool movementFrozen;

    private void FixedUpdate(){
        if(movementFrozen){
            return;
        }
        Vector3 velocity = rb.linearVelocity;
        rb.linearVelocity = new Vector3(horizontal * speed, velocity.y, vertical * speed);

        // save the user's last horizontal & vertical movement for registering the character's direction
        bool isMoving = horizontal != 0f || vertical != 0f;
        if(isMoving){
            lastHorizontal = horizontal;
            lastVertical = vertical;
        }

        Debug.Log($"[PlayerController3D] horizontal={horizontal}, vertical={vertical}, isMoving={isMoving}");
        spriteAnimator.SetActionState(isMoving ? SpriteAnimator.ActionState.Walk : SpriteAnimator.ActionState.Idle, GetDirAngle());
    }

    public float GetDirAngle() { return Mathf.Atan2(lastVertical, lastHorizontal) * Mathf.Rad2Deg; }

    // Regions allow you to lump together related code and give it a name
    #region PLAYER_CONTROLS
    public void Move(InputAction.CallbackContext context){
        horizontal = context.ReadValue<Vector2>().x;
        vertical = context.ReadValue<Vector2>().y;
        Debug.Log($"h={horizontal}, v={vertical}");
    }
    #endregion

    // freezes/unfreezes player movement, eg. while a dialogue is playing
    public void SetMovementFrozen(bool frozen){
        movementFrozen = frozen;
        if(movementFrozen){
            Vector3 velocity = rb.linearVelocity;
            rb.linearVelocity = new Vector3(0f, velocity.y, 0f);
        }
    }

    // Player can jump if grounded
    // public void Jump(InputAction.CallbackContext context){
    //     if(context.performed && IsGrounded()){
    //         rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
    //     }
    // }

    // Checks if Player is making contact with ground layer
    // private bool IsGrounded(){
    //     return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1f, 0.1f), CapsuleDirection2D.Horizontal, 0, groundLayer);
    // }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start(){

    // }

    // Update is called once per frame
    // void Update(){

    // }
}
