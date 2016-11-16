Shader "Common/Vertex Colour Detail Texture"
{
  Properties
  {
    _MainTex ("Texture", 2D) = "white" {}
  }
  SubShader
  {
    Tags { "RenderType" = "Opaque" }
    CGPROGRAM
      #pragma surface surf Lambert
      struct Input
      {
        float4 color : COLOR;
        float2 uv_MainTex;
      };

      sampler2D _MainTex;
        
      void surf (Input IN, inout SurfaceOutput o)
      {     
        o.Albedo = IN.color.rgb;
        o.Albedo *= tex2D (_MainTex, IN.uv_MainTex).rgb * 2;
      }
    ENDCG
  }
  Fallback "Diffuse"
}