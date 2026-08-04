using UnityEngine;
using UnityEngine.InputSystem; // Allows use of of Input System API

public class PlayerController3D : MonoBehaviour{

    [Header("Player Component References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] SpriteAnimator spriteAnimator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [Tooltip("The face sprite's renderer (eg. FaceAnimator's faceRenderer). Its material's _EnableWobble is kept in sync with the body's.")]
    [SerializeField] SpriteRenderer faceSpriteRenderer;

    [Header("Sprite Material")]
    [Tooltip("The sprite's currently selected material (eg. BillboardVerticalZDepth). Its _EnableWobble property is toggled via SetWobbleEnabled.")]
    [SerializeField] Material selectedMaterial;

    [Header("Player Settings")]
    [SerializeField] float speed;
    [SerializeField] float jumpingPower;

    [Header("Grounding")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheck;

    [Header("States")]
    public bool isSleepwalking;

    private static readonly int EnableWobbleID = Shader.PropertyToID("_EnableWobble");

    private float horizontal;
    private float vertical;
    private float lastHorizontal;
    private float lastVertical;
    private bool movementFrozen;
    private bool wobbleEnabled; // defaults false
    private MaterialPropertyBlock materialPropertyBlock;
    private MaterialPropertyBlock faceMaterialPropertyBlock;

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

        // wobble reflects the sleepwalking state, same as FaceAnimator forcing the eyes shut
        if(isSleepwalking != wobbleEnabled){
            SetWobbleEnabled(isSleepwalking);
        }
    }

    public float GetDirAngle() { return Mathf.Atan2(lastVertical, lastHorizontal) * Mathf.Rad2Deg; }

    // swaps the sprite's material (eg. for a different sprite look) and re-applies it
    public void SetSelectedMaterial(Material material){
        selectedMaterial = material;
        ApplySpriteMaterial();
    }

    // toggles the row-shift wobble effect on the body & face materials, without modifying the material assets themselves
    public void SetWobbleEnabled(bool enabled){
        wobbleEnabled = enabled;

        if(spriteRenderer != null){
            materialPropertyBlock ??= new MaterialPropertyBlock();
            spriteRenderer.GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetFloat(EnableWobbleID, wobbleEnabled ? 1f : 0f);
            spriteRenderer.SetPropertyBlock(materialPropertyBlock);
        }

        if(faceSpriteRenderer != null){
            faceMaterialPropertyBlock ??= new MaterialPropertyBlock();
            faceSpriteRenderer.GetPropertyBlock(faceMaterialPropertyBlock);
            faceMaterialPropertyBlock.SetFloat(EnableWobbleID, wobbleEnabled ? 1f : 0f);
            faceSpriteRenderer.SetPropertyBlock(faceMaterialPropertyBlock);
        }
    }

    private void ApplySpriteMaterial(){
        if(spriteRenderer == null) return;

        // sharedMaterial (not material) so the renderer keeps pointing at the actual asset instead of
        // Unity silently cloning it into a private runtime instance, which would disconnect live edits
        spriteRenderer.sharedMaterial = selectedMaterial;
        SetWobbleEnabled(wobbleEnabled); // preserve wobble state across material swaps
    }

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


    private void Start(){
        ApplySpriteMaterial();
        SetWobbleEnabled(isSleepwalking);
    }

    // Update is called once per frame
    // void Update(){

    // }
}
