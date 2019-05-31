Shader "Custom/LocalCulster/SimpleColor" { 
	Properties { _Color ("Main Color", Color) = (1,1,1,1)}
	SubShader { 
	Pass { 
		ZTest Always
		ZWrite Off 
		Blend One Zero 
		Lighting Off 
		Cull Off

		//Offset 1, 1 
		Color[_Color] }
}}
