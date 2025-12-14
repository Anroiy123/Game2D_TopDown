Shader "Custom/SpriteOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth ("Outline Width", Range(0, 20)) = 3
        _OutlineEnabled ("Outline Enabled", Float) = 0
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineWidth;
            float _OutlineEnabled;
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;

                // Outline logic
                if (_OutlineEnabled > 0.5)
                {
                    // Sử dụng texel size cố định để outline đều ở mọi hướng
                    float2 texelSize = _MainTex_TexelSize.xy * _OutlineWidth;

                    float maxAlpha = 0;

                    // Sample 16 điểm xung quanh theo hình tròn
                    // 4 hướng chính
                    maxAlpha = max(maxAlpha, tex2D(_MainTex, IN.texcoord + float2(texelSize.x, 0)).a);
                    maxAlpha = max(maxAlpha, tex2D(_MainTex, IN.texcoord + float2(-texelSize.x, 0)).a);
                    maxAlpha = max(maxAlpha, tex2D(_MainTex, IN.texcoord + float2(0, texelSize.y)).a);
                    maxAlpha = max(maxAlpha, tex2D(_MainTex, IN.texcoord + float2(0, -texelSize.y)).a);

                    // 4 hướng chéo
                    maxAlpha = max(maxAlpha, tex2D(_MainTex, IN.texcoord + float2(texelSize.x, texelSize.y)).a);
                    maxAlpha = max(maxAlpha, tex2D(_MainTex, IN.texcoord + float2(-texelSize.x, texelSize.y)).a);
                    maxAlpha = max(maxAlpha, tex2D(_MainTex, IN.texcoord + float2(texelSize.x, -texelSize.y)).a);
                    maxAlpha = max(maxAlpha, tex2D(_MainTex, IN.texcoord + float2(-texelSize.x, -texelSize.y)).a);

                    // Thêm 8 sample nữa để mượt hơn (xa hơn)
                    float2 texelSize2 = texelSize * 0.5;
                    maxAlpha = max(maxAlpha, tex2D(_MainTex, IN.texcoord + float2(texelSize.x, texelSize2.y)).a);
                    maxAlpha = max(maxAlpha, tex2D(_MainTex, IN.texcoord + float2(texelSize.x, -texelSize2.y)).a);
                    maxAlpha = max(maxAlpha, tex2D(_MainTex, IN.texcoord + float2(-texelSize.x, texelSize2.y)).a);
                    maxAlpha = max(maxAlpha, tex2D(_MainTex, IN.texcoord + float2(-texelSize.x, -texelSize2.y)).a);
                    maxAlpha = max(maxAlpha, tex2D(_MainTex, IN.texcoord + float2(texelSize2.x, texelSize.y)).a);
                    maxAlpha = max(maxAlpha, tex2D(_MainTex, IN.texcoord + float2(-texelSize2.x, texelSize.y)).a);
                    maxAlpha = max(maxAlpha, tex2D(_MainTex, IN.texcoord + float2(texelSize2.x, -texelSize.y)).a);
                    maxAlpha = max(maxAlpha, tex2D(_MainTex, IN.texcoord + float2(-texelSize2.x, -texelSize.y)).a);

                    // If current pixel is transparent but neighbor is not, draw outline
                    if (c.a < 0.1 && maxAlpha > 0.1)
                    {
                        c = _OutlineColor;
                    }
                }

                c.rgb *= c.a;
                return c;
            }
            ENDCG
        }
    }
}

