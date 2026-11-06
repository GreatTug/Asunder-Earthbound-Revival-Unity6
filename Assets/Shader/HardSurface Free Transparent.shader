Shader "HardSurface/Hardsurface Free/Transparent" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _SpecColor ("Specular Color", Color) = (1,1,1,1)
 _Shininess ("Shininess", Range(0.01,3)) = 1.5
 _Gloss ("Gloss", Range(0,1)) = 0.5
 _Reflection ("Reflection", Range(0,1)) = 0.5
 _Cube ("Reflection Cubemap", CUBE) = "black" {}
 _FrezPow ("Fresnel Reflection", Range(0,2)) = 0.25
 _FrezFalloff ("Fresnal/EdgeAlpha Falloff", Range(0,10)) = 4
 _EdgeAlpha ("Edge Alpha", Range(0,1)) = 1
 _Metalics ("Metalics", Range(0,1)) = 0.5
}
SubShader {
 Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
 LOD 300
 Pass {
  ColorMask 0
 }
 CGPROGRAM
#pragma surface surf BlinnPhong alpha:fade
#pragma target 3.0
 samplerCUBE _Cube;
 fixed4 _Color;
 half _Shininess;
 half _Gloss;
 half _Reflection;
 half _FrezPow;
 half _FrezFalloff;
 half _EdgeAlpha;
 half _Metalics;
 struct Input {
  float3 worldRefl;
  float3 viewDir;
 };
 void surf(Input IN, inout SurfaceOutput o) {
  fixed3 baseCol = _Color.rgb;
  o.Albedo = baseCol;
  o.Gloss = _Gloss;
  o.Specular = saturate(_Shininess * 0.333);
  half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
  half fall = pow(rim, max(_FrezFalloff, 0.01));
  half reflAmt = saturate(_Reflection + fall * _FrezPow);
  fixed3 reflCol = texCUBE(_Cube, IN.worldRefl).rgb;
  o.Emission = reflCol * reflAmt * lerp(fixed3(1,1,1), baseCol, _Metalics);
  o.Alpha = saturate(lerp(_Color.a, 1.0, fall * _EdgeAlpha) + reflAmt * 0.35);
 }
 ENDCG
}
Fallback "Transparent/Diffuse"
}