Shader "Reflective/VertexLit" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _SpecColor ("Spec Color", Color) = (1,1,1,1)
 _Shininess ("Shininess", Range(0.03,1)) = 0.7
 _ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
 _MainTex ("Base (RGB) RefStrength (A)", 2D) = "white" {}
 _Cube ("Reflection Cubemap", CUBE) = "_Skybox" {}
}
SubShader {
 Tags { "RenderType"="Opaque" }
 LOD 150
 CGPROGRAM
#pragma surface surf BlinnPhong
 sampler2D _MainTex;
 samplerCUBE _Cube;
 fixed4 _Color;
 fixed4 _ReflectColor;
 half _Shininess;
 struct Input {
  float2 uv_MainTex;
  float3 worldRefl;
 };
 void surf(Input IN, inout SurfaceOutput o) {
  fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
  o.Albedo = tex.rgb * _Color.rgb;
  o.Gloss = tex.a;
  o.Specular = _Shininess;
  fixed4 reflcol = texCUBE(_Cube, IN.worldRefl) * tex.a;
  o.Emission = reflcol.rgb * _ReflectColor.rgb;
  o.Alpha = tex.a * _Color.a;
 }
 ENDCG
}
Fallback "Diffuse"
}