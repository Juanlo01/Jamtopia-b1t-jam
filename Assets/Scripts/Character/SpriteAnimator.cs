using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField]
    public SpriteRenderer spriteRenderer;

    [SerializeField]
    public CharacterSprite characterSprite;
    [SerializeField]
    public float animationSpeed = 100f;

    private AnimationScheme.SpriteAnimation _currentSpriteAnimation;
    private float _animationTime;

    public ActionState actionState = ActionState.Idle;
    public float dirAngle = 0f;

    private string _currentDirection; // track direction name

    public string CurrentDirection => _currentDirection;
    public AnimationScheme.SpriteAnimation CurrentAnimation => _currentSpriteAnimation;
    public int CurrentFrameIndex { get; private set; }

    private static readonly int SpriteUVRectID = Shader.PropertyToID("_SpriteUVRect");
    private MaterialPropertyBlock _materialPropertyBlock;

    // fired whenever a serialized property changes in the Inspector (eg. characterSprite swapped out),
    // so listeners (eg. CharacterController3D) can force the current animation to re-resolve against it
    public event System.Action OnPropertiesChanged;

    public enum ActionState
    {
        Idle,
        Walk,
        Jump
    }

    void OnValidate()
    {
        if (spriteRenderer != null && characterSprite.sprites.Length > 0)
        {
            spriteRenderer.sprite = characterSprite.sprites[0];
        }

        OnPropertiesChanged?.Invoke();
    }
    
    // monitoring & updating current keyframe in an animation track
    void Update()
    {
        if (_currentSpriteAnimation == null || _currentSpriteAnimation.times.Length == 0)
            return;
        
        _animationTime += Time.deltaTime * animationSpeed;

        float len = _currentSpriteAnimation.length;
        float t = len > 0f ? Mathf.Repeat(_animationTime, len + 1e-6f) : 0f;

        int idx = System.Array.BinarySearch(_currentSpriteAnimation.times, t);
        if (idx < 0)
        {
            idx = ~idx - 1;
            if (idx < 0) idx = 0;
        }
        // times/frames are meant to be kept the same length, but that's only ever enforced by hand
        // (eg. in the Inspector) -- guard the lookup so a mismatch degrades gracefully instead of
        // an unchecked out-of-bounds access (which WebGL builds can't always catch as a C# exception)
        if (idx >= _currentSpriteAnimation.frames.Length) return;
        CurrentFrameIndex = idx;

        global::AnimationScheme.SpriteKeyframe frameIndex = _currentSpriteAnimation.frames[idx];

        if (characterSprite.sprites == null || characterSprite.sprites.Length == 0)
        {
            return;
        }
        if (frameIndex.index < 0 || frameIndex.index >= characterSprite.sprites.Length)
        {
            return;
        }

        Sprite sprite = characterSprite.sprites[frameIndex.index];
        spriteRenderer.sprite = sprite;
        spriteRenderer.flipX = frameIndex.flipped;

        // tell shaders (eg. the wobble pass) this frame's UV rect within the atlas, so they can remap
        // atlas-space UV back into a consistent 0-1 range local to just this one frame's cell
        Rect textureRect = sprite.textureRect;
        Texture texture = sprite.texture;
        Vector4 uvRect = new Vector4(
            textureRect.xMin / texture.width,
            textureRect.yMin / texture.height,
            textureRect.xMax / texture.width,
            textureRect.yMax / texture.height
        );

        _materialPropertyBlock ??= new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(_materialPropertyBlock);
        _materialPropertyBlock.SetVector(SpriteUVRectID, uvRect);
        spriteRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    // play animation using the current state
    public void Play(string state)
    {
        _animationTime = 0f;
        _currentSpriteAnimation = characterSprite.animationScheme.GetAnimation(state);
        Debug.Log($"[SpriteAnimator] Play(\"{state}\") -> {(_currentSpriteAnimation == null ? "NOT FOUND" : "found")}");
    }

    public void SetActionState(ActionState nextState, float angle, bool force = false)
    {
        string nextDirection = GetActionDirection("", angle);
        Debug.Log($"[SpriteAnimator] SetActionState({nextState}): angle={angle}, nextDirection={nextDirection}, current actionState={actionState}, currentDirection={_currentDirection}, force={force}");

        if (force || nextState != actionState || nextDirection != _currentDirection)
        {
            actionState = nextState;
            _currentDirection = nextDirection;

            string clipKey = actionState + _currentDirection;
            Debug.Log($"[SpriteAnimator] State/direction changed, playing clip \"{clipKey}\"");
            Play(clipKey);
        }
    }
    
    // get name of action based on direction & movement
    private string GetActionDirection(string action, float angle)
    {
        if (angle >= -45f && angle <= 45f) return action + "Left";
        if (angle > 45f && angle < 135f) return action + "Up";
        if (angle <= -135f || angle >= 135f) return action + "Right";
        return action + "Down";
    }
}
