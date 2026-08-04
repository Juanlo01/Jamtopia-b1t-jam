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
        if (serializedAnimations.Length > 0)
        {
            Animations = new Dictionary<string, SpriteAnimation>();

            for (int i = 0; i < serializedAnimations.Length; i++)
            {
                Animations.Add(serializedAnimations[i].name, serializedAnimations[i]);
            }
        }
    }
    
    public SpriteAnimation GetAnimation(string state)
    {
        return Animations[state];
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
