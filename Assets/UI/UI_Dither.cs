using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class UI_Dither : MonoBehaviour
{
    [SerializeField] private Material ditherMaterial;

    // Executes after all 3D geometry and Screen-Space Camera UI finish rendering
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (ditherMaterial != null)
        {
            // Blits the complete frame (World + UI) through the dither shader material
            Graphics.Blit(source, destination, ditherMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}