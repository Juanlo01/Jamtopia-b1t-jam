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

        spriteRenderer.sprite = characterSprite.sprites[frameIndex.index];
        spriteRenderer.flipX = frameIndex.flipped;
    }

    // play animation using the current state
    public void Play(string state)
    {
        _animationTime = 0f;
        _currentSpriteAnimation = characterSprite.animationScheme.GetAnimation(state);
        Debug.Log($"[SpriteAnimator] Play(\"{state}\") -> {(_currentSpriteAnimation == null ? "NOT FOUND" : "found")}");
    }

    public void SetActionState(ActionState nextState, float angle)
    {
        string nextDirection = GetActionDirection("", angle);
        Debug.Log($"[SpriteAnimator] SetActionState({nextState}): angle={angle}, nextDirection={nextDirection}, current actionState={actionState}, currentDirection={_currentDirection}");

        if (nextState != actionState || nextDirection != _currentDirection)
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
