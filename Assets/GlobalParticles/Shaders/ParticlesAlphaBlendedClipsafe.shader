// Upgrade NOTE: replaced 'PositionFog()' with multiply of UNITY_MATRIX_MVP by position
// Upgrade NOTE: replaced 'V2F_POS_FOG' with 'float4 pos : SV_POSITION'
// Upgrade NOTE: replaced 'glstate.matrix.modelview[0]' with 'UNITY_MATRIX_MV'

Shader "Particles/Alpha Blended Clipsafe"
{
	Properties
	{
		[PerRendererData] _MainTex("Particle Texture", 2D) = "white" {}
		_TintColor("Tint Color", Color) = (1.0,1.0,1.0,1.0)
		_MinFadeDistance("Distance of Alpha = 0", float) = 1.0
		_MaxFadeDistance("Distance of Alpha = 1", float) = 10.0
	}

	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"RenderType"="Transparent"
		}

		Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest Greater .01
		ColorMask RGB
		Lighting Off
		ZWrite Off
			Cull Off
		Fog { Color(0,0,0,0) }
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_builtin
			#pragma fragmentoption ARB_fog_exp2
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			uniform float4	_MainTex_ST;
			uniform float4 _TintColor;
			uniform float _MinFadeDistance;
			uniform float _MaxFadeDistance;

			struct appdata_vert
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			uniform sampler2D _MainTex;

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR0;
			};

			v2f vert(appdata_vert IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
				// viewPos.xyz is the vertex position in world space
				float4 viewPos = mul(UNITY_MATRIX_MV, IN.vertex);
				// _ProjectionParams.y is camera near plane
				// _ProjectionParams.z is camera far plane
				float objDist = length(ObjSpaceViewDir(IN.vertex));
				//float objDist = (-viewPos.z - _ProjectionParams.y);
				float diff = _MaxFadeDistance - _MinFadeDistance;
				float dist = objDist - _MinFadeDistance;
				float alpha = dist / diff;
				alpha = min(alpha, 1.0);
				//OUT.color = float4(IN.color.rgb, alpha);
				OUT.color = float4(IN.color.rgb, IN.color.a * alpha);
				OUT.color *= _TintColor;
				return OUT;
			}

			float4 frag(v2f IN) : COLOR
			{
				// Default
				//half4 texcol = tex2D(_MainTex, IN.uv);
				//return texcol * IN.color;

				// From Sprites/Default
				half4 c = tex2D(_MainTex, IN.uv) * IN.color;
				//c.rgb *= c.a;
				return c;
			}

			ENDCG
		}
	}

	Fallback "Particles/Alpha Blended"
}