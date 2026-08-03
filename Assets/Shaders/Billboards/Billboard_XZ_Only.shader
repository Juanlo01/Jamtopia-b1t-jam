Shader "Unlit/Billboard_XZ_Only"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" }
        Pass
        {
            ZWrite On
            Cull Off

            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };

            // helper to fetch normalized basis (rotation) columns from object->world
            void GetObjBasis(out float3 R, out float3 U, out float3 F, out float sX, out float sY, out float sZ)
            {
                // columns are the object-space axes in world space (with scale)
                float3 c0 = unity_ObjectToWorld._m00_m10_m20; // right * sX
                float3 c1 = unity_ObjectToWorld._m01_m11_m21; // up    * sY
                float3 c2 = unity_ObjectToWorld._m02_m12_m22; // fwd   * sZ

                sX = length(c0);
                sY = length(c1);
                sZ = length(c2);

                // normalized axes (pure rotation)
                R = (sX > 0.0) ? (c0 / sX) : float3(1,0,0);
                U = (sY > 0.0) ? (c1 / sY) : float3(0,1,0);
                F = (sZ > 0.0) ? (c2 / sZ) : float3(0,0,1);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.uv = v.uv;

                // object pivot in world space
                float3 objPosWS = mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;

                // original object basis + scales
                float3 objRight, objUp, objFwd;
                float sX, sY, sZ;
                GetObjBasis(objRight, objUp, objFwd, sX, sY, sZ);

                // camera-facing basis (no roll): right/up built from camera direction and world up
                float3 toCam = normalize(_WorldSpaceCameraPos.xyz - objPosWS);
                float3 worldUp = objUp; // preserve the object's own up direction (keeps Y axis fixed)
                float3 bbRight = normalize(cross(worldUp, toCam));
                // if camera is parallel to worldUp, fall back to object's right to avoid degeneracy
                bbRight = (any(isnan(bbRight)) || length(bbRight) < 1e-4) ? objRight : bbRight;
                float3 bbFwd   = normalize(cross(bbRight, worldUp)); // forward points toward camera in plane spanned by Y & cam dir

                // +90° around the (object) Y axis = yaw
                const float yawDeg = 90.0;
                float a  = radians(yawDeg);
                float ca = cos(a), sa = sin(a);

                // rotate the X–Z basis in the plane perpendicular to Y (objUp)
                float3 rotRight =  ca * bbRight + sa * bbFwd;
                float3 rotFwd   = -sa * bbRight + ca * bbFwd;

                // use rotated basis; keep Y axis unchanged
                float3 finalRight = rotRight;
                float3 finalUp    = objUp;   // unchanged (the Y axis)
                float3 finalFwd   = rotFwd;


                // apply non-uniform scale in object space along each axis
                float3 local = v.vertex.xyz;
                float3 scaled = float3(local.x * sX, local.y * sY, local.z * sZ);

                // build the world position from pivot + basis*scaled
                float3 worldPos =
                      objPosWS
                    + finalRight * scaled.x
                    + finalUp    * scaled.y
                    + finalFwd   * scaled.z;

                o.pos = mul(UNITY_MATRIX_VP, float4(worldPos, 1.0));

                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv);
                UNITY_APPLY_FOG(i.fogCoord, c);
                return c;
            }
            ENDCG
        }
    }
}
