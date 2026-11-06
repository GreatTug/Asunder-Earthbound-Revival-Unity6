Shader "HardSurface/Hardsurface Free/Transparent Specular" {
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
 _MainTex ("Diffuse(RGB) Alpha(A)", 2D) = "white" {}
 _BumpMap ("Normalmap", 2D) = "bump" {}
 _Spec_Gloss_Reflec_Masks ("Spec(R),Gloss(G) & Reflec(B) Masks ", 2D) = "white" {}
}
SubShader {
 Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
 LOD 400
 Pass {
  ColorMask 0
 }
 CGPROGRAM
#pragma surface surf BlinnPhong alpha:fade
#pragma target 3.0
 sampler2D _MainTex;
 sampler2D _BumpMap;
 sampler2D _Spec_Gloss_Reflec_Masks;
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
  float2 uv_MainTex;
  float2 uv_BumpMap;
  float3 worldRefl;
  float3 viewDir;
  INTERNAL_DATA
 };
 void surf(Input IN, inout SurfaceOutput o) {
  fixed4 tex = tex2D(_MainTex, IN.uv_MainTex) * _Color;
  fixed3 masks = tex2D(_Spec_Gloss_Reflec_Masks, IN.uv_MainTex).rgb;
  o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
  o.Albedo = tex.rgb;
  o.Gloss = _Gloss * masks.r;
  o.Specular = clamp(_Shininess * 0.333 * max(masks.g, 0.05), 0.01, 1.0);
  half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
  half fall = pow(rim, max(_FrezFalloff, 0.01));
  half reflAmt = saturate((_Reflection + fall * _FrezPow) * masks.b);
  fixed3 reflCol = texCUBE(_Cube, WorldReflectionVector(IN, o.Normal)).rgb;
  o.Emission = reflCol * reflAmt * lerp(fixed3(1,1,1), tex.rgb, _Metalics);
  o.Alpha = saturate(lerp(tex.a, 1.0, fall * _EdgeAlpha) + reflAmt * 0.35);
 }
 ENDCG
}
Fallback "Transparent/Specular"
}