Shader "Custom/Unlit/TestBlend" {

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	
	ZWrite Off
	Blend DstColor Zero
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float4 color : COLOR;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};

			fixed4 _Color;
			
			v2f vert (appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.color = IN.color;
				
				return OUT;
			}
			
			fixed4 frag (v2f IN) : SV_Target
			{
				return IN.color;
			}
		ENDCG
	}
}

}