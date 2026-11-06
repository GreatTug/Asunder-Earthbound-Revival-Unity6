Shader "Advanced SS/Bump/Reflective/SpecMap Emissive" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
 _Shininess ("Shininess", Range(0.01,1)) = 0.078125
 _ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
 _EmissiveStrength ("Emissive Strength", Float) = 1
 _MainTex ("Texture", 2D) = "white" {}
 _Cube ("Reflection Cubemap", CUBE) = "_Skybox" {}
 _BumpMap ("Bumpmap", 2D) = "bump" {}
 _SpecMap ("SpecMap (RGB)", 2D) = "white" {}
 _EmissiveMap ("EmissiveMap (RGB)", 2D) = "black" {}
}
SubShader {
 Tags { "RenderType"="Opaque" }
 LOD 400
 CGPROGRAM
#pragma surface surf BlinnPhong
#pragma target 3.0
 sampler2D _MainTex;
 sampler2D _BumpMap;
 sampler2D _SpecMap;
 sampler2D _EmissiveMap;
 samplerCUBE _Cube;
 fixed4 _Color;
 fixed4 _ReflectColor;
 half _Shininess;
 half _EmissiveStrength;
 struct Input {
  float2 uv_MainTex;
  float2 uv_BumpMap;
  float3 worldRefl;
  INTERNAL_DATA
 };
 void surf(Input IN, inout SurfaceOutput o) {
  fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
  fixed3 specMask = tex2D(_SpecMap, IN.uv_MainTex).rgb;
  fixed3 emis = tex2D(_EmissiveMap, IN.uv_MainTex).rgb * _EmissiveStrength;
  o.Albedo = tex.rgb * _Color.rgb;
  o.Gloss = specMask.r;
  o.Specular = _Shininess;
  o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
  float3 worldRefl = WorldReflectionVector(IN, o.Normal);
  fixed4 reflcol = texCUBE(_Cube, worldRefl);
  o.Emission = reflcol.rgb * _ReflectColor.rgb * specMask.b + emis;
  o.Alpha = tex.a * _Color.a;
 }
 ENDCG
}
Fallback "Bumped Specular"
}