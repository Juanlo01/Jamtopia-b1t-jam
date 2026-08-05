Shader "Unlit/BillboardVerticalZDepth"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BaseColor("Color", Color) = (1,1,1,1)
        _Emission("Emission", Range(0.0, 100.0)) = 5.0
        [Toggle] _EnableBillboard("Enable Billboarding", Float) = 1
        [Toggle] _EnableWobble("Enable Wobble Shift", Float) = 0
        _WobbleUVScale("Wobble UV Scale", Vector) = (1,1,0,0)
        _WobbleUVOffset("Wobble UV Offset", Vector) = (0,0,0,0)
        _WobbleEnvelopeCenter("Wobble Envelope Center (v)", Range(0, 1)) = 0.65
        _WobbleEnvelopeWidth("Wobble Envelope Width", Range(0.01, 1)) = 0.15
        _WobbleBaseAmplitude("Wobble Base Amplitude", Range(0, 1)) = 0.15
        _WobbleAmplitude("Wobble Amplitude (master)", Range(-1, 1)) = 0.1
        _WobbleFrequency("Wobble Frequency (v)", Float) = 1
        _SpriteUVRect("Sprite UV Rect (uMin,vMin,uMax,vMax) in atlas", Vector) = (0,0,1,1)
    }

    SubShader
    {
        Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "DisableBatching" = "True" }
        
        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            
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

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };

            sampler2D _MainTex;
            float4 _BaseColor;
            float4 _MainTex_ST;
            float _Emission;
            float _EnableBillboard;
            float _EnableWobble;
            float4 _WobbleUVScale;
            float4 _WobbleUVOffset;
            float _WobbleEnvelopeCenter;
            float _WobbleEnvelopeWidth;
            float _WobbleBaseAmplitude;
            float _WobbleAmplitude;
            float _WobbleFrequency;
            float4 _SpriteUVRect;

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
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv.xy;

                if (_EnableBillboard > 0.5)
                {
                    // billboard mesh towards camera
                    float3 vpos = mul((float3x3)unity_ObjectToWorld, v.vertex.xyz);
                    float4 worldCoord = float4(unity_ObjectToWorld._m03, unity_ObjectToWorld._m13, unity_ObjectToWorld._m23, 1);
                    float4 viewPos = mul(UNITY_MATRIX_V, worldCoord) + float4(vpos, 0);

                    o.pos = mul(UNITY_MATRIX_P, viewPos);

                    // calculate distance to vertical billboard plane seen at this vertex's screen position
                    float3 planeNormal = normalize(float3(UNITY_MATRIX_V._m20, 0.0, UNITY_MATRIX_V._m22));
                    float3 planePoint = unity_ObjectToWorld._m03_m13_m23;
                    float3 rayStart = _WorldSpaceCameraPos.xyz;
                    float3 rayDir = -normalize(mul(UNITY_MATRIX_I_V, float4(viewPos.xyz, 1.0)).xyz - rayStart); // convert view to world, minus camera pos
                    float dist = rayPlaneIntersection(rayDir, rayStart, planeNormal, planePoint);

                    // calculate the clip space z for vertical plane
                    float4 planeOutPos = mul(UNITY_MATRIX_VP, float4(rayStart + rayDir * dist, 1.0));
                    float newPosZ = planeOutPos.z / planeOutPos.w * o.pos.w;

                    // use the closest clip space z
                    #if defined(UNITY_REVERSED_Z)
                    o.pos.z = max(o.pos.z, newPosZ);
                    #else
                    o.pos.z = min(o.pos.z, newPosZ);
                    #endif
                }

                UNITY_TRANSFER_FOG(o,o.pos);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                if (_EnableWobble > 0.5)
                {
                    // uv is in atlas space (this frame's own small sub-rect within the sheet), which
                    // differs per frame/row - remap it into a 0-1 range local to just this frame first,
                    // so the wobble reads the same regardless of where this frame sits in the atlas
                    float2 rectMin = _SpriteUVRect.xy;
                    float2 rectMax = _SpriteUVRect.zw;
                    float2 localUV = (uv - rectMin) / (rectMax - rectMin);

                    // remap this sprite's own local uv into the shared "body" uv space, so a smaller/offset
                    // sprite (eg. eyes) can be tuned to ride the same wobble phase as the body sprite
                    float2 wobbleUV = localUV * _WobbleUVScale.xy + _WobbleUVOffset.xy;

                    // single smooth (non-piecewise) bump: peaks at _WobbleEnvelopeCenter, tapers down to
                    // _WobbleBaseAmplitude away from it, sized by _WobbleEnvelopeWidth - this only shapes
                    // how strongly each row wobbles RELATIVE to the peak, it doesn't control overall intensity
                    float dist = wobbleUV.y - _WobbleEnvelopeCenter;
                    float envelope = _WobbleBaseAmplitude + (1 - _WobbleBaseAmplitude) * exp(-(dist * dist) / (2 * _WobbleEnvelopeWidth * _WobbleEnvelopeWidth));

                    // shift each row of pixels along u, animated over time; _WobbleFrequency controls how many
                    // wave cycles fit across v, _WobbleAmplitude is the master intensity (0 = no wobble at all)
                    float t = _Time.y;
                    float u_disp = cos(wobbleUV.y * _WobbleFrequency - t);
                    uv.x += u_disp * envelope * _WobbleAmplitude;
                }

                fixed4 text = tex2D(_MainTex, uv);
                fixed4 modu = text * _BaseColor;
                fixed3 emit = modu.rgb * _Emission;
                fixed4 col  = fixed4(emit, modu.a);
                UNITY_APPLY_FOG(i.fogCoord, col);

                return col;
            }

            ENDCG
        }
    }
}