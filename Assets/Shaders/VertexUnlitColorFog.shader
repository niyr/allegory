﻿Shader "Unlit/VertexColorFog" { // http://docs.unity3d.com/Manual/SL-VertexProgramInputs.html
	SubShader{
		Pass{

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog

#include "UnityCG.cginc"

		// vertex input: position, color
	struct appdata {
		float4 vertex : POSITION;
		fixed4 color : COLOR;
	};

	struct v2f {
		float4 pos : SV_POSITION;
		UNITY_FOG_COORDS(1)
		fixed4 color : COLOR;
	};

	v2f vert(appdata v) {
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.color = v.color;
		UNITY_TRANSFER_FOG(o, o.pos);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target{ 
		UNITY_APPLY_FOG(i.fogCoord, i.color);
		return i.color; 
	}
		ENDCG
	}
	}
}