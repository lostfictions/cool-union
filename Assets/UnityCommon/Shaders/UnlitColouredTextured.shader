Shader "Common/UnlitColouredTextured" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100
		Lighting Off
		Pass {
			SetTexture [_MainTex] {
				constantColor [_Color]
				Combine texture * constant, texture * constant
			}
		}
	}
}