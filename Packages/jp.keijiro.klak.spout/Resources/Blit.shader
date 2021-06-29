Shader "Hidden/Klak/Spout/Blit"
{
    Properties
    {
        _MainTex("", 2D) = "white" {}
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;

    void Vertex(float4 position : POSITION,
                float2 texCoord : TEXCOORD0,
                out float4 outPosition : SV_Position,
                out float2 outTexCoord : TEXCOORD0)
    {
        outPosition = UnityObjectToClipPos(position);
        outTexCoord = float2(texCoord.x, 1 - texCoord.y);
    }

    float4 BlitSimple(float4 position : SV_Position,
                      float2 texCoord : TEXCOORD0) : SV_Target
    {
        return tex2D(_MainTex, texCoord);
    }

    float4 BlitClearAlpha(float4 position : SV_Position,
                          float2 texCoord : TEXCOORD0) : SV_Target
    {
        return float4(tex2D(_MainTex, texCoord).rgb, 1);
    }

    float4 BlitToLinear(float4 position : SV_Position,
                        float2 texCoord : TEXCOORD0) : SV_Target
    {
        float4 c = tex2D(_MainTex, texCoord);
        c.rgb = GammaToLinearSpace(c.rgb);
        return c;
    }

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment BlitSimple
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment BlitClearAlpha
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment BlitToLinear
            ENDCG
        }
    }
}
