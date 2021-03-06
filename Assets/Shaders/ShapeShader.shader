﻿Shader "Custom/Unlit/Transparent/ShapeShader" {
Properties {
	_MainTex ("Texture", 2D) = "white" {}
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				float4 color : COLOR;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
				OUT.color = IN.color;
				return OUT;
			}
			
			float4 frag (v2f IN) : SV_Target
			{
				float4 color = tex2D(_MainTex, IN.texcoord) * IN.color;
				return color;
			}
		ENDCG
	}
}

}
