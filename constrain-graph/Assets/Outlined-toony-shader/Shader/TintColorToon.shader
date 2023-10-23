Shader "Custom/TintColorToon"
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

	}
		SubShader
		{
			Tags
			{
				"RenderType" = "Opaque"
				"Queue" = "Geometry"
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

					o.pos = UnityObjectToClipPos(v.vertex);
					o.worldNormal = UnityObjectToWorldNormal(v.normal);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					TRANSFER_SHADOW(o)
					return o;
				}



				float4 frag(v2f i) : SV_Target
				{
					float3 normal = normalize(i.worldNormal);
					float shadow = SHADOW_ATTENUATION(i) * dot(_WorldSpaceLightPos0, normal);
					float light = step(0.1, shadow);
					float omLight = 1 - light;
					float4 mainTexture = tex2D(_MainTex, i.uv);
					float4 tint = mainTexture.r * _TintR + mainTexture.b * _TintB + mainTexture.g * _TintG;
					return (omLight * _ShadowColor + light) * _Color * tint * _Brightness;
				}
				ENDCG
			}

			//Shadow casting support.
			UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
		}
}
