using UnityEngine;
using UnityEngine.InputSystem; // Allows use of of Input System API

public class PlayerController : MonoBehaviour{

    // Puts a old heading above a group of variables in Inspector
    [Header("Player Component References")]
    // SerializeField keeps variable private while exposing it in Inspector
    [SerializeField] Rigidbody2D rb;
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

    private void FixedUpdate(){
        rb.linearVelocity = new Vector2(horizontal * speed, vertical * speed);

        // save the user's last horizontal & vertical movement for registering the character's direction
        bool isMoving = horizontal != 0f || vertical != 0f;
        if(isMoving){
            lastHorizontal = horizontal;
            lastVertical = vertical;
        }

        Debug.Log($"[PlayerController] horizontal={horizontal}, vertical={vertical}, isMoving={isMoving}");
        spriteAnimator.SetActionState(isMoving ? SpriteAnimator.ActionState.Walk : SpriteAnimator.ActionState.Idle, GetDirAngle());
    }
    
    public float GetDirAngle() { return Mathf.Atan2(lastVertical, lastHorizontal) * Mathf.Rad2Deg; }
}
