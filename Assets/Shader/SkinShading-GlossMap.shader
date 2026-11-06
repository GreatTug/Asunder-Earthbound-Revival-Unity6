Shader "Advanced SS/Skin/Skin Shading (Gloss Map)" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _BumpMap ("Bumpmap", 2D) = "bump" {}
 _SpecMap ("Gloss (R), Shininess (G)", 2D) = "white" {}
 _Kspec ("Ks", Float) = 0.45
 _Kdiffuse ("Kd", Float) = 1
 _Kambient ("Ka", Float) = 0.2
 _ColourIntensity ("ColourIntensity", Range(1,2.2)) = 1.69
 _ShadowStrength ("ShadowStrength", Range(0,1)) = 0.94
 _NormalBlend ("NormalBlend", Range(0,2)) = 1.22
 _RedWeight ("RedWeight", Range(-0.1,0.3)) = 0
 _Shininess ("Shininess", Range(0.01,1)) = 0.74
}
SubShader {
 Tags { "RenderType"="Opaque" }
 LOD 400
 CGPROGRAM
#pragma surface surf SkinWrap fullforwardshadows
#pragma target 3.0

 sampler2D _MainTex;
 sampler2D _BumpMap;
 sampler2D _SpecMap;
 fixed4 _Color;
 half _Kspec;
 half _Kdiffuse;
 half _Kambient;
 half _ColourIntensity;
 half _ShadowStrength;
 half _NormalBlend;
 half _RedWeight;
 half _Shininess;

 struct Input {
  float2 uv_MainTex;
  float2 uv_BumpMap;
 };

 half4 LightingSkinWrap(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
  half NdotL = dot(s.Normal, lightDir);
  half diff = saturate(NdotL * 0.75 + 0.25);
  half redScatter = saturate(NdotL * 0.5 + 0.5) * max(_RedWeight, 0.0);
  half shadowAtten = lerp(1.0, atten, _ShadowStrength);
  half3 h = normalize(lightDir + viewDir);
  half nh = max(0.0, dot(s.Normal, h));
  half spec = pow(nh, max(s.Specular * 128.0, 4.0)) * s.Gloss * _Kspec;
  half4 c;
  half3 scatterTint = half3(diff + redScatter, diff, diff);
  c.rgb = (s.Albedo * _LightColor0.rgb * scatterTint * _Kdiffuse + _LightColor0.rgb * spec) * shadowAtten;
  c.a = s.Alpha;
  return c;
 }

 void surf(Input IN, inout SurfaceOutput o) {
  fixed4 tex = tex2D(_MainTex, IN.uv_MainTex) * _Color;
  fixed4 specTex = tex2D(_SpecMap, IN.uv_MainTex);
  o.Albedo = tex.rgb * (_ColourIntensity * 0.6);
  o.Emission = o.Albedo * (_Kambient * 0.25);
  half3 n = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
  o.Normal = normalize(lerp(half3(0,0,1), n, saturate(_NormalBlend * 0.65)));
  o.Gloss = specTex.r;
  o.Specular = max(0.02, _Shininess * max(specTex.g, 0.2));
  o.Alpha = tex.a;
 }
 ENDCG
}
Fallback "Bumped Specular"
}