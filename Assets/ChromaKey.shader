Shader "Unlit/ChromaKey"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
		{
			Tags { "RenderType" = "Transparent" }

			// No culling or depth
			Cull Off ZWrite Off ZTest Always

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					float2 uv : TEXCOORD0;
				};

				float4 _LightColor0;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;

					return o;
				}

				sampler2D _MainTex;

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv);
					//col.a = 0;
				
				    //clip(255 - col.g);
					col = fixed4(col.r, col.b, 0, col.a);

					clip((col.r > 0.2 || col.b > 0.2) ? 1 : -1);

					return col;
				}

			ENDCG
			}
		}
}