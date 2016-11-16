Shader "Common/UnlitColoured" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,0)

	}
    SubShader {
		Tags { "RenderType"="Opaque" }
        Pass { Color[_Color] }
    }
}
