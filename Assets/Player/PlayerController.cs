using UnityEngine;
using UnityEngine.InputSystem; // Allows use of of Input System API

public class PlayerController : MonoBehaviour{

    // Puts a old heading above a group of variables in Inspector
    [Header("Player Component References")]
    // SerializeField keeps variable private while exposing it in Inspector
    [SerializeField] Rigidbody2D rb;

    [Header("Player Settings")]
    [SerializeField] float speed;
    [SerializeField] float jumpingPower;

    [Header("Grounding")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheck;

    private float horizontal;
    private float vertical;

    private void FixedUpdate(){
        rb.linearVelocity = new Vector2(horizontal * speed, vertical * speed);
    }

    // Regions allow you to lump together related code and give it a name
    #region PLAYER_CONTROLS     
    public void Move(InputAction.CallbackContext context){
        horizontal = context.ReadValue<Vector2>().x;
        vertical = context.ReadValue<Vector2>().y;
    }
    #endregion

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
