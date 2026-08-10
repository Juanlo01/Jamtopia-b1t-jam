using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceAnimator : MonoBehaviour
{
    [Header("Connections")]
    [SerializeField] private SpriteAnimator spriteAnimator;
    [SerializeField] private SpriteRenderer faceRenderer;
    [SerializeField] private PlayerController3D playerController;

    [Header("Face Sprites")]
    [SerializeField] private Sprite downFaceSprite_Open;
    [SerializeField] private Sprite downFaceSprite_Partial;
    [SerializeField] private Sprite downFaceSprite_Closed;
    [SerializeField] private Sprite leftFaceSprite_Open;
    [SerializeField] private Sprite leftFaceSprite_Partial;
    [SerializeField] private Sprite leftFaceSprite_Closed;
    [SerializeField] private Sprite rightFaceSprite_Open;
    [SerializeField] private Sprite rightFaceSprite_Partial;
    [SerializeField] private Sprite rightFaceSprite_Closed;

    [Header("Blinking")]
    [Tooltip("Duration of a blink animation")]
    public float eyeCloseTime = 0.15f;
    [Tooltip("Minimum time between blink calls")]
    public float minEyeCloseInterval = 2f;
    [Tooltip("Maximum time between blink calls")]
    public float maxEyeCloseInterval = 6f;

    [Header("Position")]
    public Vector3 baseOffset;

    [Tooltip("Set of offset settings for how face sprite should move per-animation")]
    public List<FaceOffsetSettings> offsetSettings = new List<FaceOffsetSettings>();

    private EyeState _eyeState = EyeState.Open;
    private Coroutine _blinkLoopRoutine;
    private Coroutine _closeEyesRoutine;

    public enum FaceDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    private enum EyeState
    {
        Open,
        Partial,
        Closed
    }

    [System.Serializable]
    public class FaceOffsetSettings
    {
        public SpriteAnimator.ActionState state;
        public FaceDirection direction;
        public float xOffset;
        public float yOffset;
        public float xRange;
        public float yRange;
        public int xPhase;
        public int yPhase;
    }

    void Reset()
    {
        offsetSettings.Clear();
        foreach (SpriteAnimator.ActionState state in new[] { SpriteAnimator.ActionState.Idle, SpriteAnimator.ActionState.Walk })
        {
            foreach (FaceDirection direction in System.Enum.GetValues(typeof(FaceDirection)))
            {
                offsetSettings.Add(new FaceOffsetSettings { state = state, direction = direction });
            }
        }
    }

    void OnEnable()
    {
        _eyeState = EyeState.Open;
        _blinkLoopRoutine = StartCoroutine(BlinkLoop());
    }

    void OnDisable()
    {
        if (_blinkLoopRoutine != null) StopCoroutine(_blinkLoopRoutine);
        if (_closeEyesRoutine != null) StopCoroutine(_closeEyesRoutine);
        _blinkLoopRoutine = null;
        _closeEyesRoutine = null;
        _eyeState = EyeState.Open;
    }

    void Update()
    {
        if (spriteAnimator == null) return;

        if (playerController != null && playerController.isSleepwalking)
        {
            if (_closeEyesRoutine != null)
            {
                StopCoroutine(_closeEyesRoutine);
                _closeEyesRoutine = null;
            }
            _eyeState = EyeState.Closed;
        }

        FaceDirection direction = ParseDirection(spriteAnimator.CurrentDirection);
        FaceOffsetSettings settings = GetSettings(spriteAnimator.actionState, direction);

        float xOffset = settings != null ? settings.xOffset : 0f;
        float yOffset = settings != null ? settings.yOffset : 0f;
        float xRange = settings != null ? settings.xRange : 0f;
        float yRange = settings != null ? settings.yRange : 0f;
        int xPhase = settings != null ? settings.xPhase : 0;
        int yPhase = settings != null ? settings.yPhase : 0;

        int frameCount = spriteAnimator.CurrentAnimation != null ? spriteAnimator.CurrentAnimation.frames.Length : 0;
        int frameIndex = spriteAnimator.CurrentFrameIndex;
        float xWave = ComputeWave(frameIndex, frameCount, xPhase);
        float yWave = ComputeWave(frameIndex, frameCount, yPhase);

        float x = baseOffset.x + xOffset + xWave * (xRange * 0.5f);
        float y = baseOffset.y + yOffset + yWave * (yRange * 0.5f);

        transform.localPosition = new Vector3(x, y, baseOffset.z);

        UpdateFaceSprite(direction);
    }

    // blink animation
    public void CloseEyes()
    {
        if (playerController != null && playerController.isSleepwalking) return; // eyes stay closed, no blinking

        if (_closeEyesRoutine != null) StopCoroutine(_closeEyesRoutine);
        _closeEyesRoutine = StartCoroutine(CloseEyesRoutine());
    }

    private IEnumerator BlinkLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minEyeCloseInterval, maxEyeCloseInterval));
            CloseEyes();
        }
    }

    private IEnumerator CloseEyesRoutine()
    {
        float segment = eyeCloseTime / 4f;

        _eyeState = EyeState.Open;
        yield return new WaitForSeconds(segment);

        _eyeState = EyeState.Partial;
        yield return new WaitForSeconds(segment);

        _eyeState = EyeState.Closed;
        yield return new WaitForSeconds(segment);

        _eyeState = EyeState.Partial;
        yield return new WaitForSeconds(segment);

        _eyeState = EyeState.Open;

        _closeEyesRoutine = null;
    }

    // computes where a face sprite offset is at a given frame and phase during the animation
    private float ComputeWave(int frameIndex, int frameCount, int phase)
    {
        if (frameCount <= 0) return 0f;
        int shifted = ((frameIndex - phase) % frameCount + frameCount) % frameCount;
        float progress = shifted / (float)frameCount;
        return Mathf.Sin(progress * Mathf.PI * 2f);
    }

    private FaceOffsetSettings GetSettings(SpriteAnimator.ActionState state, FaceDirection direction)
    {
        foreach (FaceOffsetSettings settings in offsetSettings)
        {
            if (settings.state == state && settings.direction == direction) return settings;
        }
        return null;
    }

    private void UpdateFaceSprite(FaceDirection direction)
    {
        if (faceRenderer == null) return;

        if (direction == FaceDirection.Up)
        {
            faceRenderer.enabled = false;
            return;
        }
        faceRenderer.enabled = true;

        Sprite sprite = (direction, _eyeState) switch
        {
            (FaceDirection.Down, EyeState.Open) => downFaceSprite_Open,
            (FaceDirection.Down, EyeState.Partial) => downFaceSprite_Partial,
            (FaceDirection.Down, EyeState.Closed) => downFaceSprite_Closed,
            (FaceDirection.Left, EyeState.Open) => leftFaceSprite_Open,
            (FaceDirection.Left, EyeState.Partial) => leftFaceSprite_Partial,
            (FaceDirection.Left, EyeState.Closed) => leftFaceSprite_Closed,
            (FaceDirection.Right, EyeState.Open) => rightFaceSprite_Open,
            (FaceDirection.Right, EyeState.Partial) => rightFaceSprite_Partial,
            (FaceDirection.Right, EyeState.Closed) => rightFaceSprite_Closed,
            _ => null
        };

        if (sprite != null) faceRenderer.sprite = sprite;
    }

    private FaceDirection ParseDirection(string direction)
    {
        switch (direction)
        {
            case "Up": return FaceDirection.Up;
            case "Left": return FaceDirection.Left;
            case "Right": return FaceDirection.Right;
            default: return FaceDirection.Down;
        }
    }
}
