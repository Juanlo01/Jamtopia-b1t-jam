using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationScheme", menuName = "ScriptableObjects/AnimationScheme")]
public class AnimationScheme : ScriptableObject
{
    [SerializeField]
    public SpriteAnimation[] serializedAnimations; // animations set within scriptable objects
    
    protected Dictionary<string, SpriteAnimation> Animations; // reformatted animations got by spriteanimator

    void OnValidate()
    {
        BuildAnimationsLookup();
    }

    // OnValidate is editor-only and never runs in a build, so Animations would otherwise stay null
    // forever at runtime (this is what caused the WebGL build to crash on its very first animation
    // state change) -- build it lazily here too, so it works whether or not OnValidate ever ran.
    private void BuildAnimationsLookup()
    {
        Animations = new Dictionary<string, SpriteAnimation>();

        foreach (SpriteAnimation animation in serializedAnimations)
        {
            Animations[animation.name] = animation;
        }
    }

    public SpriteAnimation GetAnimation(string state)
    {
        if (Animations == null) BuildAnimationsLookup();
        return Animations.TryGetValue(state, out SpriteAnimation animation) ? animation : null;
    }
    
    [System.Serializable]
    public class SpriteKeyframe
    {
        public int index;
        public bool flipped;
    }
    
    [System.Serializable]
    public class SpriteAnimation
    {
        public string name;
        public float[] times; // sorted ascending keytimes
        public SpriteKeyframe[] frames; // same length as Keys
        public float length; // last keytime
        
        public static SpriteAnimation FromMap((float time, int frame, bool flipped)[] keyframes)
        {
            var keys = new List<float>();
            var frames = new List<SpriteKeyframe>();

            foreach (var (time, frame, flipped) in keyframes)
            {
                keys.Add(time);
                frames.Add(new SpriteKeyframe { index = frame, flipped = flipped });
            }

            // Ensure ascending time order
            var combined = keys
                .Select((k, i) => new { Time = k, Frame = frames[i] })
                .OrderBy(x => x.Time)
                .ToList();

            return new SpriteAnimation
            {
                times = combined.Select(x => x.Time).ToArray(),
                frames = combined.Select(x => x.Frame).ToArray(),
                length = combined.Count > 0 ? combined[^1].Time : 0f
            };
        }
    }
}
