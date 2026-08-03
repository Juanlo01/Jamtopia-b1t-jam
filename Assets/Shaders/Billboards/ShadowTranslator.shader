Shader "Unlit/ShadowTranslator"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BaseColor("Color", Color) = (1,1,1,1)
        _Emission("Emission", Range(0.0, 100.0)) = 5.0
        _ControllerHeight("Controller Height", Range(0.0,10.0)) = 2.0
        _AngleOffsetDeg("Angle Offset Deg", Range(-180,180)) = 0.0
    }

    SubShader
    {
        Tags{ "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "DisableBatching"="True" }

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; };
            struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; UNITY_FOG_COORDS(1) };

            sampler2D _MainTex;
            float4 _MainTex_ST, _BaseColor;
            float  _Emission;
            float  _ControllerHeight;
            float  _AngleOffsetDeg;

            v2f vert(appdata v)
            {
                v2f o;

                // Object origin in world space
                float3 objWorldPos = unity_ObjectToWorld._m03_m13_m23;

                // Radius (your latest spec)
                float R = _ControllerHeight * 0.25;

                // Direction from object to camera, flattened to XZ
                float3 toCamWS = _WorldSpaceCameraPos.xyz - objWorldPos;
                toCamWS.y = 0.0;

                // Fallback if camera is straight above/below
                if (dot(toCamWS, toCamWS) < 1e-8)
                {
                    float3 camFwdWS = mul(UNITY_MATRIX_I_V, float4(0,0,-1,0)).xyz;
                    toCamWS = float3(camFwdWS.x, 0.0, camFwdWS.z);
                }

                // We want the object to move OPPOSITE the camera direction
                float3 dirXZ = normalize(-toCamWS);

                // Optional yaw-only rotation around +Y (no extra PI now)
                float yaw = radians(_AngleOffsetDeg) + UNITY_PI;
                float s, c; sincos(yaw, s, c);
                float3x3 rotY = float3x3(
                    c,   0, -s,
                    0,   1,  0,
                    s,   0,  c
                );
                float3 targetDir = normalize(mul(rotY, dirXZ));

                // New object position: original + away-from-camera * radius
                float3 newObjPos = objWorldPos + targetDir * R;

                // Rigid translation of all vertices
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                worldPos.xyz += (newObjPos - objWorldPos);

                o.pos = mul(UNITY_MATRIX_VP, worldPos);
                o.uv  = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }


            fixed4 frag(v2f i):SV_Target
            {
                fixed4 tex  = tex2D(_MainTex, i.uv);
                fixed4 modu = tex * _BaseColor;
                fixed3 emit = modu.rgb * _Emission;
                fixed4 col  = fixed4(emit, modu.a);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
