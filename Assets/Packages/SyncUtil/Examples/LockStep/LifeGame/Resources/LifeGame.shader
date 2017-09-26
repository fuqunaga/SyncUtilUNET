Shader "LifeGame/Visualize"
{
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "./Data.cginc"

			StructuredBuffer<Data> _Buf;

			fixed4 frag (v2f_img i) : SV_Target
			{
				int2 xy = int2(_Width,_Height) * i.uv;
				Data data = _Buf[xyToIdx(xy)];
				return fixed4((data.alive ? 1 : 0.5).xxx, 1);
			}
			ENDCG
		}
	}
}
