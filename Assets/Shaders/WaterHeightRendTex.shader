Shader "CustomRenderTexture/WaterHeightRendTex"
{
    Properties
    {
     
    }
    SubShader
    {
        Lighting Off
        Blend One Zero

        Pass
        {
            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 3.0

            // https://docs.unity3d.com/Packages/com.unity.shadergraph@12.0/manual/Gradient-Noise-Node.html
            float2 unity_gradientNoise_dir(float2 p)
            {
                p = p % 289;
                float x = (34 * p.x + 1) * p.x % 289 + p.y;
                x = (34 * x + 1) * x % 289;
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }

            float unity_gradientNoise(float2 p)
            {
                float2 ip = floor(p);
                float2 fp = frac(p);
                float d00 = dot(unity_gradientNoise_dir(ip), fp);
                float d01 = dot(unity_gradientNoise_dir(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(unity_gradientNoise_dir(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(unity_gradientNoise_dir(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
            }

            float Unity_GradientNoise_float(float2 UV, float Scale)
            {
                return unity_gradientNoise(UV * Scale) + 0.5;
            }

            float4 frag (v2f_customrendertexture IN) : COLOR
            {
                float h = Unity_GradientNoise_float(float2((_Time.y / 100.0),(_Time.y / 100.0)) + IN.globalTexcoord.xy, 20.0) * 0.7;
                float4 col = float4(h, h, h, 1.0);
                // float4 col = sin(_Time);
                return col;
            }
            ENDCG
        }
    }
}