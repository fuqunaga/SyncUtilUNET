Shader "GPUFluid/FloatVisualize"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;

			fixed4 frag (v2f_img i) : SV_Target
			{
				//return any(tex2D(_MainTex, i.uv) < 0) ? fixed4(1,0,0,1) : fixed4(0,0,0,1);
				return (tex2D(_MainTex, i.uv) + (1).xxxx) * 0.5;
			}
			ENDCG
		}
	}
}
