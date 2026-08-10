// Transparent Sprite2D shader with three mutually-exclusive visual states,
// selected at runtime via _State (0 = NONE, 1 = OUTLINE, 2 = HOVER).
// Built directly on top of URP's own Sprite-Unlit-Default.shader vertex
// stage (flip/instancing/SpriteRenderer.color plumbing) so it drops in as a
// swap-in replacement without breaking sprite atlases or batching; only the
// fragment stage differs.
Shader "Custom/Sprite2D Highlight"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        [MaterialToggle] _ZWrite("ZWrite", Float) = 0

        [Space]
        [Enum(NONE,0,OUTLINE,1,HOVER,2)] _State ("State", Float) = 0
        _AlphaThreshold ("Alpha Cutoff", Range(0, 1)) = 0.5
        _OutlineThickness ("Outline Thickness (px)", Range(0, 8)) = 1
        _OutlineThickness2 ("Second Outline Thickness (px)", Range(0, 8)) = 1
        _PulseSpeed ("Pulse Speed", Range(0.1, 10)) = 2

        // Legacy properties, kept so materials gracefully fall back if swapped to Sprites/Default.
        [HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Off
        ZWrite [_ZWrite]

        Pass
        {
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

            #pragma vertex HighlightVertex
            #pragma fragment HighlightFragment

            struct Attributes
            {
                COMMON_2D_INPUTS
                half4 color : COLOR;
                UNITY_SKINNED_VERTEX_INPUTS
            };

            struct Varyings
            {
                COMMON_2D_OUTPUTS
                half4 color : COLOR;
            };

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/2DCommon.hlsl"

            #pragma multi_compile_instancing
            #pragma multi_compile _ DEBUG_DISPLAY SKINNED_SPRITE

            // NOTE: Do not ifdef the properties here as SRP batcher can not handle different layouts.
            // _State/_AlphaThreshold/_OutlineThickness/_PulseSpeed are ordinary per-material properties;
            // when several sprites share one material, override them per-renderer with a
            // MaterialPropertyBlock (SpriteRenderer.SetPropertyBlock) rather than editing the material.
            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                float4 _MainTex_TexelSize;
                float _State;
                float _AlphaThreshold;
                float _OutlineThickness;
                float _OutlineThickness2;
                float _PulseSpeed;
            CBUFFER_END

            Varyings HighlightVertex(Attributes input)
            {
                UNITY_SKINNED_VERTEX_COMPUTE(input);
                SetUpSpriteInstanceProperties();
                input.positionOS = UnityFlipSprite(input.positionOS, unity_SpriteProps.xy);

                Varyings o = CommonUnlitVertex(input);
                o.color = input.color * _Color * unity_SpriteColor;
                return o;
            }

            // Oscillates between 0.0 and 1.0 brightness; used by both OUTLINE and HOVER states.
            half Pulse()
            {
                return sin(_Time.y * _PulseSpeed) * 0.5h + 0.5h;
            }

            // Highest alpha found in a ring of samples `thicknessPx` texels away from uv.
            // Used to detect silhouette edges without needing a second (pre-baked) outline texture.
            half SilhouetteNeighborAlpha(float2 uv, float thicknessPx)
            {
                float2 px = _MainTex_TexelSize.xy * thicknessPx;

                half maxAlpha = 0;
                maxAlpha = max(maxAlpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2( px.x,     0)).a);
                maxAlpha = max(maxAlpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-px.x,     0)).a);
                maxAlpha = max(maxAlpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(    0, px.y)).a);
                maxAlpha = max(maxAlpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(    0,-px.y)).a);
                maxAlpha = max(maxAlpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2( px.x, px.y)).a);
                maxAlpha = max(maxAlpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-px.x, px.y)).a);
                maxAlpha = max(maxAlpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2( px.x,-px.y)).a);
                maxAlpha = max(maxAlpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-px.x,-px.y)).a);
                return maxAlpha;
            }

            half4 HighlightFragment(Varyings input) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                half4 albedo = tex * input.color;

                // NONE: unmodified sprite.
                if (_State < 0.5)
                {
                    return albedo;
                }

                half b = Pulse();

                // Shared by OUTLINE and HOVER: two pulsing rims drawn into the transparent
                // pixels bordering the silhouette. The first rim sits directly against the
                // sprite; the second wraps around the first, one thickness further out, and
                // always pulses with the inverse brightness of the first.
                if (tex.a <= _AlphaThreshold)
                {
                    if (SilhouetteNeighborAlpha(input.uv, _OutlineThickness) > _AlphaThreshold)
                    {
                        return half4(b, b, b, input.color.a);
                    }

                    if (SilhouetteNeighborAlpha(input.uv, _OutlineThickness + _OutlineThickness2) > _AlphaThreshold)
                    {
                        half inverse = 1.0h - b;
                        return half4(inverse, inverse, inverse, input.color.a);
                    }

                    return albedo;
                }

                // OUTLINE: sprite itself renders normally inside the rims.
                if (_State < 1.5)
                {
                    return albedo;
                }

                // HOVER: every non-transparent pixel is replaced by a flat, pulsing grey/white fill.
                return half4(b, b, b, tex.a * input.color.a);
            }
            ENDHLSL
        }
    }
}
