Shader "Custom/Enemy"
{
    Properties
    {
        // 设置shader参数
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        [Toggle]
        _FlipX ("FlipX", Int) = 0
        [Toggle]
        _SetFixedColor ("SetFixedColor", Int) = 0
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
                        
            #pragma vertex vert
            #pragma fragment frag
            
            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _SetFixedColor;
            float _FlipX;
            

            struct a2v
            {
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                // uv
                float2 uv : TEXCOORD0;
            };
            
            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;
                if (_FlipX)
                {
                    o.uv.x = 1 - o.uv.x;
                }
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //采贴图颜色
                fixed4 textureColor = tex2D(_MainTex, i.uv);
                //_SetFixedColor为Ture时直接设色
                if (_SetFixedColor)
                {
                    return _Color;
                }
                
                return textureColor * _Color;
            }
            
            ENDCG
        }
    }
}
