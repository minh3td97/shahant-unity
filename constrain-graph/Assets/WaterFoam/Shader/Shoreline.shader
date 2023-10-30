Shader "Custom/Shoreline"
{
    Properties
    {
        _DeepColor ("Deep Color", Color) = (1, 1, 1, 1)
        _ShallowColor ("Shallow Color", Color) = (1, 1, 1, 1)
        
        // Fade length
        _DepthFactor ("Depth Factor", float) = 1
        // Fade Smoothness
        _DepthPow ("Depth Power", float) = 0.5
        // Color of the shoreline foam
        _FoamColor ("Foam Color", Color) = (1, 1, 1, 1)
        // Shoreline Strenght
        _ShorelinePow ("Shoreline Strength", float) = 2
        // Shoreline Size
        _ShorelineThresh ("Shoreline Size", float) = 0.1

        [NoScaleOffset] _NormalMap ("Normal Map", 2D) = "bump" {}
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _DeepColor;
            float4 _ShallowColor;
            float _DepthFactor;
            float _DepthPow;
            float4 _FoamColor;
            float _ShorelinePow;
            float _ShorelineThresh;

            sampler2D _NormalMap;
            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
            //sampler2D FoamMap;


            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXTCOORD1;
                float4 tangent : TEXTCOORD2;
                float4 bitangent : TEXTCOORD3;
                float4 screenPos : TEXTCOORD4;
                float4 viewVector : TEXTCOORD5;
                float3 worldPos : TEXTCOORD6;
                float3 worldNormal : TEXTCOORD7;
            };


            v2f vert (MeshData v)
            {
                v2f o;

                
                /*
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.tangent = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
                o.bitangent = float4(cross(o.normal.xyz, o.tangent) * (v.tangent.w * unity_WorldTransformParams.w), v.tangent.w);
                
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.screenPos = ComputeScreenPos(v.vertex);
                o.worldNormal = normalize(mul(unity_ObjectToWorld, float4(v.normal, 0)).xyz);
                
                */

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                COMPUTE_EYEDEPTH(o.screenPos.z);
                //o.uv = v.uv.xy;

                //o.screenPos = ComputeScreenPos(v.vertex);

                // Lastly, pass the vertex, after wave modulation
                //o.vertex = UnityObjectToClipPos(v.vertex);

                //float3 viewVector = mul(unity_CameraInvProjection, float4((o.screenPos.xy / o.screenPos.w) * 2 - 1, 0, -1));
				//o.viewVector = mul(unity_CameraToWorld, float4(viewVector, 0));




                return o;
            }


            float4 frag (v2f i) : SV_Target
            {
               
                // Find the water depth (zBuffer - distance)
                float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)));
                float depth = sceneZ - i.screenPos.z;

                // Fade the water
                float depthFade = saturate((abs(pow(depth, _DepthPow))) / _DepthFactor);
                float4 color = lerp(_ShallowColor, _DeepColor, depthFade);

                // Find the shoreline intersection
                float intersection = saturate((abs(depth)) / _ShorelineThresh);
                
                // Add the foam color on the intersection
                float shoreline = 1 - intersection;
                color += _FoamColor * pow(shoreline, 4) * _ShorelinePow * 2;
                color.a = intersection;


                return color;
                /*
                // Applies Normal Map to Normals
                float3 tangentSpaceNormal = UnpackNormal(tex2D(_NormalMap, i.uv));
                float3x3 mtxTangentToWorld = {
                    i.tangent.x, i.bitangent.x, i.normal.x,
                    i.tangent.y, i.bitangent.y, i.normal.y,
                    i.tangent.z, i.bitangent.z, i.normal.z
                };
                float3 normal = mul(mtxTangentToWorld, tangentSpaceNormal);


                float nonLinearDepth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, i.screenPos);
				float depth = LinearEyeDepth(nonLinearDepth);
				float dstToWater = i.screenPos.w;
				float waterViewDepth = depth - dstToWater;

                float waterAlpha = 1;
                float colDepthFactor = 4;
                float3 waterColor = lerp(_ShallowColor, _DeepColor, 1 - exp(-waterViewDepth * colDepthFactor));
                


                return float4(waterColor, waterAlpha);
                */
            }

            ENDCG
        }
    }
}
