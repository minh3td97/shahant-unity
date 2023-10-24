Shader "Shahant/TintOutlineToon"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_TintR("TintR", Color) = (1,1,1,1)
		_TintB("TintB", Color) = (1,1,1,1)
		_TintG("TintG", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
		_ShadowColor("Shadow Color", Color) = (0.92,0.92,0.92,1)
		_Brightness("Brightness", Float) = 1.0

		[Space(10)]
		_OutlineValue("Outline Value", Float) = 0.2
		[Toggle(OVERRIDE_OUTLINE_COLOR)] _OverrideOutlineColor("Override Outline Color", Float) = 0
		_OutlineColor("Outline Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags
		{
			"RenderType"="Opaque"
			"Queue"="Geometry"
		}
		
		Pass
		{
			
			Cull Front
			ZTest Less

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature OVERRIDE_OUTLINE_COLOR

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;	
				float3 normal : NORMAL;
				float3 smoothNormal : TEXCOORD3;
				float4 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 position : SV_POSITION;
				fixed4 color : COLOR;
				float2 uv  : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _OutlineColor;
			float _OutlineValue;
			float4 _Color;

			inline half GetMaxComponent(half3 inColor)
			{
				return max(max(inColor.r, inColor.g), inColor.b);
			}

			inline half3 SetSaturation(half3 inColor, half inSaturation)
			{
	
				half maxComponent = GetMaxComponent(inColor) - 0.0001;
				half3 saturatedColor = step(maxComponent.rrr, inColor) * inColor;
				return lerp(inColor, saturatedColor, inSaturation);
			}


			v2f vert (appdata input)
			{
				v2f output;

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 normal = any(input.smoothNormal) ? input.smoothNormal : input.normal;
				float3 viewPosition = UnityObjectToViewPos(input.vertex);
				float3 viewNormal = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, normal));

				output.position = UnityViewToClipPos(viewPosition + viewNormal * -viewPosition.z * _OutlineValue / 1000.0);
				output.color = _OutlineColor;

				return output;
			}

			#define SATURATION_FACTOR 0.6
			#define BRIGHTNESS_FACTOR 0.8

			float4 frag (v2f i) : SV_Target
			{
				#ifdef OVERRIDE_OUTLINE_COLOR
				return _OutlineColor;
				#else
				half4 mainMapColor = tex2D(_MainTex, i.uv);
	
				half3 outlineColor = BRIGHTNESS_FACTOR 
					* SetSaturation(mainMapColor.rgb, SATURATION_FACTOR)
					* mainMapColor.rgb;
	
				return half4(outlineColor, mainMapColor.a) * _Color;
				
				#endif

			}
			ENDCG
		}
		Pass
		{
			
			Cull Back
			ZTest LEqual

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct appdata
			{
				float4 vertex : POSITION;				
				float4 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 worldNormal : NORMAL;
				float2 uv : TEXCOORD0;
				float3 viewDir : TEXCOORD1;
				fixed4 diff : COLOR4;
				SHADOW_COORDS(2)
					
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float4 _ShadowColor;
			float _Brightness;
			float4 _TintR;
			float4 _TintB;
			float4 _TintG;

			v2f vert(appdata v)
			{
				v2f o;

				float3 worldNormal = UnityObjectToWorldNormal(v.normal);

				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldNormal = worldNormal;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.viewDir = WorldSpaceViewDir(v.vertex);

				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				o.diff = nl * _LightColor0;
				o.diff.rgb += ShadeSH9(half4(worldNormal, 1));

				TRANSFER_SHADOW(o)

					return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float4 mainTexture = tex2D(_MainTex, i.uv);
				float4 tint = mainTexture.r * _TintR + mainTexture.b * _TintB + mainTexture.g * _TintG;

				float3 normal = normalize(i.worldNormal);
				float shadow = SHADOW_ATTENUATION(i);
				float shadowDotNormal = shadow * dot(_WorldSpaceLightPos0, normal);
				shadow = step(0.1, shadowDotNormal);

				tint.rgb = lerp(tint.rgb, _ShadowColor, (1 - shadow) * _ShadowColor.a);

				return tint;
			}
				ENDCG
		}

		// Shadow casting support.
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}
}