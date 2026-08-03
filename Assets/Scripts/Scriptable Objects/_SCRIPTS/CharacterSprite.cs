using UnityEngine;

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
        sprites = Resources.LoadAll<Sprite>("Textures/Characters/"+spriteTexture.name);
    }
}
