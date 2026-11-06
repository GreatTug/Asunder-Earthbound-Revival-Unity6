Shader "HardSurface/Hardsurface Free/Opaque Bump" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _SpecColor ("Specular Color", Color) = (1,1,1,1)
 _Shininess ("Shininess", Range(0.01,3)) = 1.5
 _Gloss ("Gloss", Range(0,1)) = 0.5
 _Reflection ("Reflection", Range(0,1)) = 0.5
 _Cube ("Reflection Cubemap", CUBE) = "black" {}
 _FrezPow ("Fresnel Reflection", Range(0,2)) = 0.25
 _FrezFalloff ("Fresnal/EdgeAlpha Falloff", Range(0,10)) = 4
 _EdgeAlpha ("Edge Alpha", Range(0,1)) = 0
 _Metalics ("Metalics", Range(0,1)) = 0.5
 _MainTex ("Diffuse(RGB) Alpha(A)", 2D) = "white" {}
 _BumpMap ("Normalmap", 2D) = "bump" {}
}
SubShader {
 Tags { "Queue"="Geometry" "IgnoreProjector"="False" "RenderType"="Opaque" }
 LOD 300
 CGPROGRAM
#pragma surface surf BlinnPhong
#pragma target 3.0
 sampler2D _MainTex;
 sampler2D _BumpMap;
 samplerCUBE _Cube;
 fixed4 _Color;
 half _Shininess;
 half _Gloss;
 half _Reflection;
 half _FrezPow;
 half _FrezFalloff;
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
  o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
  o.Albedo = tex.rgb * (1.0 - _Reflection * 0.5 * _Metalics);
  o.Gloss = _Gloss;
  o.Specular = saturate(_Shininess * 0.333);
  half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
  half frez = pow(rim, max(_FrezFalloff, 0.01)) * _FrezPow;
  half reflAmt = saturate(_Reflection + frez);
  fixed3 reflCol = texCUBE(_Cube, WorldReflectionVector(IN, o.Normal)).rgb;
  o.Emission = reflCol * reflAmt * lerp(fixed3(1,1,1), tex.rgb, _Metalics);
  o.Alpha = 1.0;
 }
 ENDCG
}
Fallback "Bumped Specular"
}