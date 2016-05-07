

Shader "RedCartel/Unlit/TintedMaskShader"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_MaskTex("Mask Texture (RGB)", 2D) = "white" {}
	}
	SubShader
	{
		Tags {
          "Queue"="Transparent"
          "IgnoreProjector"="False"
          "RenderType"="Transparent"
		}
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv_mask : TEXCOORD1;
				fixed4 color : COLOR; // Vertex color
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv_mask : TEXCOORD1;
				UNITY_FOG_COORDS(2) // Use TEXCOORD1 for fog so as not to conflict with 1 above
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR; // Vertex color
			};

			float4 _Color;
			sampler2D _MainTex;
			sampler2D _MaskTex;
			
			// _ST suffix Required by TRANSFORM_TEX - A unity specific MACRO for tiling and offset
			float4 _MainTex_ST;
			float4 _MaskTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv_mask = TRANSFORM_TEX(v.uv_mask, _MaskTex);
				o.color = _Color; // remove from vertex color to input color
				
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o; 
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 outputCol = tex2D(_MainTex, i.uv) * i.color;
				
				outputCol.a = outputCol.a * (tex2D(_MaskTex, i.uv_mask)).r;
				
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, outputCol);
								
				return outputCol;
			}
			ENDCG
		}
	}
}
