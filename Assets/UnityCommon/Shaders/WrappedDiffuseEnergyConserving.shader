Shader "Common/Wrapped Diffuse (Energy-Conserving)" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_WrapFactor ("Wrap Factor", Range(0, 1.0)) = 0.5
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 200

CGPROGRAM
#pragma surface surf ConservingWrappedLambert

sampler2D _MainTex;
fixed4 _Color;
half _WrapFactor;

half4 LightingConservingWrappedLambert (SurfaceOutput s, half3 lightDir, half atten) {
	half wrappedDiffuse = saturate((dot(s.Normal, lightDir) + _WrapFactor) / ((1 + _WrapFactor) * (1 + _WrapFactor)));
    half4 c;
    c.rgb = s.Albedo * _LightColor0.rgb * (wrappedDiffuse * atten * 2);
    c.a = s.Alpha;
    return c;
}

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	o.Albedo = c.rgb;
	o.Alpha = c.a;
}
ENDCG
}

Fallback "VertexLit"
}
