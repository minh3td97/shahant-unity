Shader "Shahant/TintColorToon"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
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
				"LightMode" = "ForwardBase"
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
					SHADOW_COORDS(2)
					fixed4 diff : COLOR4;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _Color;
				float4 _ShadowColor;
				float _Brightness;

				v2f vert(appdata v)
				{
					v2f o;

					float3 worldNormal = UnityObjectToWorldNormal(v.normal);

					o.pos = UnityObjectToClipPos(v.vertex);
					o.worldNormal = worldNormal;
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);

					half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
					o.diff = nl * _LightColor0;
					o.diff.rgb += ShadeSH9(half4(worldNormal, 1));

					TRANSFER_SHADOW(o)

					return o;
				}

				float4 frag(v2f i) : SV_Target
				{
					// Color
					float4 mainTexture = tex2D(_MainTex, i.uv);
					float4 tint = mainTexture * _Color;

					// Light
					half3 nonLightColor = _Color;
					float lightFactorByColor = length(tint.rgb - nonLightColor) / 1.73205;

					tint = lerp(tint, tint * i.diff, lightFactorByColor);

					// Shadow
					float shadow = SHADOW_ATTENUATION(i);
					tint.rgb = lerp(tint.rgb, _ShadowColor, (1 - shadow) * _ShadowColor.a);

					return tint;
				}
				ENDCG
			}

			//Shadow casting support.
			UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
		}
}
