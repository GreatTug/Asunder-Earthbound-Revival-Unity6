Shader "Self-Illumin/VertexLit" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _SpecColor ("Spec Color", Color) = (1,1,1,1)
 _Shininess ("Shininess", Range(0.1,1)) = 0.7
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Illum ("Illumin (A)", 2D) = "white" {}
 _EmissionLM ("Emission (Lightmapper)", Float) = 0
}
SubShader {
 Tags { "RenderType"="Opaque" }
 LOD 100
 CGPROGRAM
#pragma surface surf BlinnPhong
 sampler2D _MainTex;
 sampler2D _Illum;
 fixed4 _Color;
 half _Shininess;
 struct Input {
  float2 uv_MainTex;
  float2 uv_Illum;
 };
 void surf(Input IN, inout SurfaceOutput o) {
  fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
  fixed4 c = tex * _Color;
  o.Albedo = c.rgb;
  o.Emission = c.rgb * tex2D(_Illum, IN.uv_Illum).a;
  o.Gloss = tex.a;
  o.Specular = _Shininess;
  o.Alpha = c.a;
 }
 ENDCG
}
Fallback "Diffuse"
}