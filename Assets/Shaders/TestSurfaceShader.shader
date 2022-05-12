Shader "Custom/TestSurfaceShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Outline ("out line", Range(0,1)) = 0.0
    }
    SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200
        
            cull front
            CGPROGRAM
            #pragma surface surf no vertex:vert noshadow
            #pragma target 3.0

            sampler2D _MainTex;
            float _Outline;

            struct Input
            {
                float2 color:COLOR;
            };

            half _Glossiness;
            half _Metallic;
            fixed4 _Color;

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_INSTANCING_BUFFER_END(Props)
                
            void surf(Input IN, inout SurfaceOutput o) {}
            /*
            void surf(Input IN, inout SurfaceOutput o)
            {
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
                //o.Emission = c.rgb;
                o.Albedo = c.rgb;
            }
            */
            void vert(inout appdata_full v)
            {
                //v.vertex.y = v.vertex.y + sin(_Time.y);
                v.vertex.xyz = v.vertex.xyz + v.normal * _Outline;
            }

            float4 Lightingno(SurfaceOutput s, float3 lightDir, float atten)
            {
                return float4(0, 0, 0, 1);
            }
            /*
            float4 LightingHelp(SurfaceOutput s, float3 lightDir, float atten)
            {
                float NdotL = dot(s.Normal, lightDir) * 0.5 + 0.5;
                NdotL = NdotL * NdotL * NdotL;
                //NdotL = saturate(NdotL);
                
                float4 FinalColor;
                //FinalColor.rgb = s.Albedo * NdotL *_LightColor0.rgb * atten;
                FinalColor.rgb = s.Albedo * NdotL * atten;
                FinalColor.a = s.Alpha;

                return FinalColor;
            }
            */
     
        ENDCG
            cull back
            CGPROGRAM

            #pragma surface surf Lambert vertex:vert addshadow
            #pragma target 3.0

            sampler2D _MainTex;

            struct Input
            {
                float2 uv_MainTex;
            };

            half _Glossiness;
            half _Metallic;
            fixed4 _Color;

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_INSTANCING_BUFFER_END(Props)

                void surf(Input IN, inout SurfaceOutput o)
            {
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
                //o.Emission = c.rgb;
                o.Albedo = c.rgb;
            }

            void vert(inout appdata_full v)
            {
                //v.vertex.y = v.vertex.y + sin(_Time.y);
            }
            /*
            float4 LightingHelp(SurfaceOutput s, float3 lightDir, float atten)
            {
                float NdotL = dot(s.Normal, lightDir) * 0.5 + 0.5;
                NdotL = NdotL * NdotL * NdotL;
                //NdotL = saturate(NdotL);

                float4 FinalColor;
                //FinalColor.rgb = s.Albedo * NdotL *_LightColor0.rgb * atten;
                FinalColor.rgb = s.Albedo * NdotL * atten;
                FinalColor.a = s.Alpha;

                return FinalColor;
            }
            */
        ENDCG
    }
    FallBack "Diffuse"
}
