Shader "Sprites/Magic"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_AddTex("Additive (RGB)", 2D) = "white" {}
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Global }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile DUMMY PIXELSNAP_ON
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 uv		: TEXCOORD0;
				float3 normal	: NORMAL;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color	: COLOR;
				float2 uv		: TEXCOORD0;
				float2 cap		: TEXCOORD1;
			};

			uniform float4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.uv = IN.uv;
				OUT.color = IN.color * _Color;
		#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
		#endif

				float3 worldNorm = normalize(_World2Object[0].xyz * IN.normal.x + _World2Object[1].xyz * IN.normal.y + _World2Object[2].xyz * IN.normal.z);
				worldNorm = mul((float3x3)UNITY_MATRIX_V, worldNorm);
				OUT.cap.xy = worldNorm.xy * 0.5 + 0.5;

				return OUT;
			}

			uniform sampler2D _MainTex;
			uniform sampler2D _AddTex;

			fixed4 frag(v2f IN) : SV_Target
			{
				/*
				fixed4 c = tex2D(_MainTex, IN.uv) * IN.color;
				c.rgb *= c.a;
				fixed4 add = tex2D(_AddTex, IN.cap);
				return (c + (add * 2.0) - 1.0);
				*/
				/*
				fixed4 tex = tex2D(_MainTex, IN.uv);
				fixed4 mc = tex2D(_AddTex, IN.cap);

				return (tex + (mc*2.0) - 1.0);
				*/

				fixed4 c = tex2D(_MainTex, IN.uv) * IN.color;
				fixed4 add = tex2D(_AddTex, IN.cap);
				c.a -= add.r;

				//c.rgb *= c.a;
				return (c);
			}

			ENDCG
		}
	}
}