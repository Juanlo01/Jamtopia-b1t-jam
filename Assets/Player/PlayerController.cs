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
}
