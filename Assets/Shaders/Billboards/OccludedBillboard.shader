Shader "Unlit/OccludedBillboard"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BaseColor("Color", Color) = (1,1,1,1)
        _Emission("Emission", Range(0.0, 100.0)) = 5.0
        _DitherTileSize("DitherTileSize", Range(0.0, 64.0)) = 0.1
    }

    SubShader
    {
        Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "DisableBatching" = "True" }
        
        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            ZTest Greater
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
                float4 scrPos : TEXCOORD1;   // +++
                UNITY_FOG_COORDS(2)
            };

            sampler2D _MainTex;
            float4 _BaseColor;
            float4 _MainTex_ST;
            float _Emission;
            float _DitherTileSize;

            float rayPlaneIntersection( float3 rayDir, float3 rayPos, float3 planeNormal, float3 planePos)
            {
                float denom = dot(planeNormal, rayDir);
                denom = max(denom, 0.000001); // avoid divide by zero
                float3 diff = planePos - rayPos;
                return dot(diff, planeNormal) / denom;
            }

            v2f vert(appdata v)
            {
                v2f o;

                // Seed (keeps a defined w before we overwrite o.pos)
                float4 objClip = UnityObjectToClipPos(v.vertex);
                o.pos = objClip;
                o.uv  = v.uv;

                // --- Billboard mesh toward camera (your original) ---
                float3 vpos       = mul((float3x3)unity_ObjectToWorld, v.vertex.xyz);
                float4 worldCoord = float4(unity_ObjectToWorld._m03, unity_ObjectToWorld._m13, unity_ObjectToWorld._m23, 1);
                float4 viewPos    = mul(UNITY_MATRIX_V, worldCoord) + float4(vpos, 0);

                // Project to clip
                o.pos = mul(UNITY_MATRIX_P, viewPos);

                // --- Vertical plane depth adjustment (your original) ---
                float3 planeNormal = normalize(float3(UNITY_MATRIX_V._m20, 0.0, UNITY_MATRIX_V._m22));
                float3 planePoint  = unity_ObjectToWorld._m03_m13_m23;
                float3 rayStart    = _WorldSpaceCameraPos.xyz;
                float3 rayDir      = -normalize(mul(UNITY_MATRIX_I_V, float4(viewPos.xyz, 1.0)).xyz - rayStart);
                float  dist        = rayPlaneIntersection(rayDir, rayStart, planeNormal, planePoint);

                float4 planeOutPos = mul(UNITY_MATRIX_VP, float4(rayStart + rayDir * dist, 1.0));
                float  newPosZ     = planeOutPos.z / planeOutPos.w * o.pos.w;

                #if defined(UNITY_REVERSED_Z)
                    o.pos.z = max(o.pos.z, newPosZ);
                #else
                    o.pos.z = min(o.pos.z, newPosZ);
                #endif

                // Screen-space position for pixel-accurate dithering (do this AFTER final o.pos)
                o.scrPos = ComputeScreenPos(o.pos);

                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }


            fixed4 frag(v2f i) : SV_Target
            {
                // Sample the texture
                fixed4 text = tex2D(_MainTex, i.uv);
                fixed4 modu = text * _BaseColor;
                fixed3 emit = modu.rgb * _Emission;
                fixed4 col  = fixed4(emit, modu.a);

                // Apply fog to the emissive color
                UNITY_APPLY_FOG(i.fogCoord, col);

                // --- Screen-space dithering ---
                // Convert to [0,1] screen UV
                float2 uvSS  = i.scrPos.xy / i.scrPos.w;
                // Convert to pixel coords
                float2 pixel = uvSS * _ScreenParams.xy;

                // Checkerboard: size in pixels
                const float cellSize = _DitherTileSize; // tweak as needed
                float2 cell   = frac(pixel / cellSize);
                float dither  = step(0.5, cell.x) * step(0.5, cell.y);

                // Alpha = visible only if texture has alpha and passes dither
                float colored = text.a > 0.0 ? _Emission : 0.0;
                return fixed4(1.0, 1.0, 1.0, colored * dither);
            }

            ENDCG
        }
    }
}