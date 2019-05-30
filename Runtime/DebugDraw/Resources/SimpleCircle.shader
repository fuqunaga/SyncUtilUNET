Shader "Custom/SyncUtil/SimpleCircle" { 
	Properties { 
		_Color ("Main Color", Color) = (1,1,1,1)
	}

	SubShader { 
		Tags{ "RenderType" = "Translate" "PreviewType" = "Plane" }

		Pass { 
			ZTest Always
			ZWrite Off 
			Blend SrcAlpha OneMinusSrcAlpha 
			Lighting Off 
			Cull Off


			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			half4 _Color;

			fixed4 frag(v2f_img i) : SV_Target
			{
				half alpha = smoothstep(0, 0.05, 1 - length(i.uv.xy -0.5)*2);
				return fixed4(_Color.rgb, _Color.a * alpha);
			}
			ENDCG
		}
	}
}
