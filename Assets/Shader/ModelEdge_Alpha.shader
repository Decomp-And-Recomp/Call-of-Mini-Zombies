Shader "Triniti/Model/ModelEdge_Alpha" 
{
    Properties
    {
        _MainTex ("Texture (RGB)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _AtmoColor ("Atmosphere Color", Color) = (0.5,0.5,1,1)
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" }
        Pass
        {
            Name "PLANETBASE"
            Tags { "LIGHTMODE"="Always" "QUEUE"="Transparent" }
            BindChannels {
            Bind "vertex", Vertex
            Bind "normal", Normal
            Bind "texcoord", TexCoord0
            }
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float4 _MainTex_ST;
            sampler2D _MainTex;
            float4 _Color;
            float4 _AtmoColor;
            struct appdata_t
            {
                float4 texcoord0 : TEXCOORD0;
                float3 normal : NORMAL;
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float texcoord3 : TEXCOORD3;
                float2 texcoord2 : TEXCOORD2;
                float3 texcoord1 : TEXCOORD1;
                float3 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                float3 tmpvar_1;
                float4 tmpvar_2;
                tmpvar_2.w = 0.000000;
                tmpvar_2.xyz = normalize(v.normal);
                float3 tmpvar_3;
                tmpvar_3 = normalize(mul((float3x3)UNITY_MATRIX_MV, tmpvar_2.xyz));

                float tmpvar_4;
                tmpvar_4 = clamp ((((tmpvar_3.x * tmpvar_3.x) + (tmpvar_3.y * tmpvar_3.y)) - ((tmpvar_3.z * tmpvar_3.z) * 0.500000)), 0.000000, 1.00000);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord0 = tmpvar_3;
                o.texcoord1 = tmpvar_1;
                o.texcoord2 = ((v.texcoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
                o.texcoord3 = (tmpvar_4 * tmpvar_4);
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 texColor = tex2D(_MainTex, i.texcoord2);
                return lerp(texColor * _Color, _AtmoColor, i.texcoord3);
            }
            ENDCG
        }
    }
    
}