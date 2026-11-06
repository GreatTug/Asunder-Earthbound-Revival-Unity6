Shader "Hidden/BlurAndFlares" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "" {}
 _NonBlurredTex ("Base (RGB)", 2D) = "" {}
}
CGINCLUDE
#include "UnityCG.cginc"
sampler2D _MainTex;
sampler2D _NonBlurredTex;
float4 _Offsets;
float4 offsets;
float4 _TintColor;
float4 tintColor;
float4 _Threshhold;
float _StretchWidth;
float stretchWidth;
float _Saturation;

float2 GetOff() { return _Offsets.xy + offsets.xy; }
float4 GetTint() { return (dot(_TintColor, _TintColor) > 0.0) ? _TintColor : tintColor; }
float GetStretch() { return (_StretchWidth > 0.0) ? _StretchWidth : stretchWidth; }

half4 fragCopy(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 return tex2D(_MainTex, i.uv);
}

half4 fragStretch(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 float2 d = GetOff() * GetStretch();
 half4 b = tex2D(_MainTex, i.uv);
 b = max(b, tex2D(_MainTex, i.uv + d * 1.0));
 b = max(b, tex2D(_MainTex, i.uv - d * 1.0));
 b = max(b, tex2D(_MainTex, i.uv + d * 2.0));
 b = max(b, tex2D(_MainTex, i.uv - d * 2.0));
 return b;
}

half4 fragPreFilter(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 half4 color = tex2D(_MainTex, i.uv);
 color.rgb = max(half3(0,0,0), color.rgb - _Threshhold.xxx);
 half lum = Luminance(color.rgb);
 color.rgb = lerp(lum.xxx, color.rgb, _Saturation > 0.0 ? _Saturation : 1.0);
 return color * GetTint();
}

half4 fragPreMax(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 float2 d = GetOff() * 0.002;
 half4 c = tex2D(_MainTex, i.uv);
 c = max(c, tex2D(_MainTex, i.uv + d));
 c = max(c, tex2D(_MainTex, i.uv - d));
 return c;
}

half4 fragGaussBlur(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 float2 d = GetOff();
 half4 color = tex2D(_MainTex, i.uv) * 0.227027027;
 color += tex2D(_MainTex, i.uv + d * 1.38461538) * 0.316216216;
 color += tex2D(_MainTex, i.uv - d * 1.38461538) * 0.316216216;
 color += tex2D(_MainTex, i.uv + d * 3.23076923) * 0.070270270;
 color += tex2D(_MainTex, i.uv - d * 3.23076923) * 0.070270270;
 return color;
}
ENDCG

SubShader {
 ZTest Always Cull Off ZWrite Off
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragCopy
ENDCG }
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragStretch
ENDCG }
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragPreFilter
ENDCG }
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragPreMax
ENDCG }
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragGaussBlur
ENDCG }
}
Fallback Off
}