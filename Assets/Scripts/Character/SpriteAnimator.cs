using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    private SpriteRenderer _sr;
    private PlayerController _ch;
    
    [SerializeField]
    public CharacterSprite characterSprite;
    [SerializeField]
    public float animationSpeed = 100f;

    private AnimationScheme.SpriteAnimation _currentSpriteAnimation;
    private float _animationTime;

    public ActionState actionState = ActionState.Idle;
    public float dirAngle = 0f;
    
    private string _currentDirection; // just track direction name, not action
    
    public enum ActionState
    {
        Idle,
        Walk,
        Jump
    }
    
    void Awake()
    {
        // GET COMPONENTS
        _sr = GetComponent<SpriteRenderer>();
        _ch = GetComponent<PlayerController>();
    }

    void OnValidate()
    {
        Awake();
        if (characterSprite.sprites.Length > 0)
        {
            _sr.sprite = characterSprite.sprites[0];
        }
    }
    
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

        global::AnimationScheme.SpriteKeyframe frameIndex = _currentSpriteAnimation.frames[idx];

        if (characterSprite.sprites == null || characterSprite.sprites.Length == 0)
        {
            return;
        }
        if (frameIndex.index < 0 || frameIndex.index >= characterSprite.sprites.Length)
        {
            return;
        }

        _sr.sprite = characterSprite.sprites[frameIndex.index];
        _sr.flipX = frameIndex.flipped;
    }

    public void Play(string state)
    {
        _animationTime = 0f;
        _currentSpriteAnimation = characterSprite.animationScheme.GetAnimation(state);
    }

    public void SetActionState(ActionState nextState)
    {
        string nextDirection = GetActionDirection("", _ch.GetDirAngle()); // returns "Up", "Left", etc.

        // Only play if the action changes OR the direction changes
        if (nextState != actionState || nextDirection != _currentDirection)
        {
            actionState = nextState;
            _currentDirection = nextDirection;

            string clipKey = actionState + _currentDirection; // e.g., "IdleUp"
            Play(clipKey);
        }
    }
    
    private string GetActionDirection(string action, float angle)
    {
        if (angle > 0 && angle < 90) return action + "Up";
        if (angle >= 90 && angle <= 180) return action + "Left";
        if (angle > 180 && angle < 270) return action + "Down";
        return action + "Right";
    }
}
