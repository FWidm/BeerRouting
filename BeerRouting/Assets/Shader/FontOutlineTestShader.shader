Shader "GUI/Outline Text Shader"
{
	Properties
	{
		_MainTex("Font Texture", 2D) = "white" {}
	_Color("Text Color", Color) = (1,1,1,1)
		_OutlineColor("Outline Color", Color) = (120,120,120,255)
		_Outline("Outline width", Range(0, 1)) = 0
	}

		SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Lighting Off Cull Off ZTest off ZWrite Off Fog{ Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

	struct appdata_t {
		float4 vertex : POSITION;
		float2 texcoord : texcoord;
	};

	struct v2f {
		float4 vertex : POSITION;
		float2 texcoord : texcoord;
	};

	sampler2D _MainTex;
	uniform float4 _MainTex_ST;
	uniform float4 _Color;
	uniform float4 _OutlineColor;
	uniform float _Outline;

	v2f vert(appdata_t v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP , v.vertex);
		o.texcoord = TRANSFORM_TEX(v.texcoord ,_MainTex);
		return o;
	}

	float4 frag(v2f i) : COLOR
	{
		float4 col = _Color;
		float width = _Outline;
		if (tex2D(_MainTex, i.texcoord).a < 1 && tex2D(_MainTex, i.texcoord).a > 0)
		{
			col = _OutlineColor;
		}
		col.a *= tex2D(_MainTex, i.texcoord).a * 100;
		return col;
	}
		ENDCG
	}
	}
		Fallback "GUI/Text Shader"
}