
Shader "Shahant/AlphaBlendDynamic" {
    Properties{
        _MainTex("MainTex", 2D) = "white" {}
        _TintColor("Color", Color) = (0.5,0.5,0.5,1)
        _Speed("Speed", Float) = 1
        [HideInInspector]_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
    }
        SubShader{
            Tags {
                "IgnoreProjector" = "True"
                "Queue" = "Transparent"
                "RenderType" = "Transparent"
            }
            Pass {
                Name "FORWARD"
                Tags {
                    "LightMode" = "ForwardBase"
                }
                Blend SrcAlpha OneMinusSrcAlpha
                ZWrite Off

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_instancing
                #include "UnityCG.cginc"
                #pragma multi_compile_fwdbase
                #pragma multi_compile_fog
                #pragma target 3.0
                uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
                UNITY_INSTANCING_BUFFER_START(Props)
                    UNITY_DEFINE_INSTANCED_PROP(float4, _TintColor)
                    UNITY_DEFINE_INSTANCED_PROP(float, _Speed)
                UNITY_INSTANCING_BUFFER_END(Props)
                struct VertexInput {
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                    float4 vertex : POSITION;
                    float2 texcoord0 : TEXCOORD0;
                    float4 vertexColor : COLOR;
                };
                struct VertexOutput {
                    float4 pos : SV_POSITION;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                    float2 uv0 : TEXCOORD0;
                    float4 vertexColor : COLOR;
                    UNITY_FOG_COORDS(1)
                };
                VertexOutput vert(VertexInput v) {
                    VertexOutput o = (VertexOutput)0;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_TRANSFER_INSTANCE_ID(v, o);
                    o.uv0 = v.texcoord0;
                    o.vertexColor = v.vertexColor;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    UNITY_TRANSFER_FOG(o,o.pos);
                    return o;
                }
                float4 frag(VertexOutput i) : COLOR {
                    UNITY_SETUP_INSTANCE_ID(i);
                ////// Lighting:
                ////// Emissive:
                                float _Speed_var = UNITY_ACCESS_INSTANCED_PROP(Props, _Speed);
                                float4 node_8192 = _Time;
                                float2 node_1885 = (i.uv0 + (_Speed_var * node_8192.g) * float2(1,0));
                                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_1885, _MainTex));
                                float4 _TintColor_var = UNITY_ACCESS_INSTANCED_PROP(Props, _TintColor);
                                float3 emissive = (_MainTex_var.rgb * i.vertexColor.rgb * _TintColor_var.rgb * 2.0);
                                float3 finalColor = emissive;
                                fixed4 finalRGBA = fixed4(finalColor,(_MainTex_var.a * i.vertexColor.a * _TintColor_var.a));
                                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                                return finalRGBA;
                            }
                            ENDCG
                        }
        }
            
}
