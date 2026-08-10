using UnityEngine;
#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "CharacterSprite", menuName = "ScriptableObjects/CharacterSprite")]
public class CharacterSprite : ScriptableObject
{
    [SerializeField]
    private Texture spriteTexture;

    [SerializeField]
    public AnimationScheme animationScheme;

    public Sprite[] sprites;

    void OnValidate()
    {
        if (!spriteTexture) return;
#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(spriteTexture);
        sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(path).OfType<Sprite>().ToArray();
        Debug.Log($"[CharacterSprite] Loaded {sprites.Length} sprite(s) from \"{path}\"");
#endif
    }
}
