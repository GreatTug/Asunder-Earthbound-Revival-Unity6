Shader "Hidden/BlendModesOverlay" {
Properties {
 _MainTex ("Screen Blended", 2D) = "" {}
 _Overlay ("Color", 2D) = "grey" {}
}
CGINCLUDE
#include "UnityCG.cginc"
sampler2D _MainTex;
sampler2D _Overlay;
float4 _MainTex_TexelSize;
half _Intensity;

struct v2f_o {
 float4 pos : SV_POSITION;
 float2 uv0 : TEXCOORD0;
 float2 uv1 : TEXCOORD1;
 UNITY_VERTEX_OUTPUT_STEREO
};

v2f_o vertO(appdata_img v) {
 v2f_o o;
 UNITY_INITIALIZE_OUTPUT(v2f_o, o);
 UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
 o.pos = UnityObjectToClipPos(v.vertex);
 o.uv0 = v.texcoord.xy;
 o.uv1 = v.texcoord.xy;
 #if UNITY_UV_STARTS_AT_TOP
 if (_MainTex_TexelSize.y < 0)
  o.uv0.y = 1.0 - o.uv0.y;
 #endif
 return o;
}

half4 fragAdditive(v2f_o i) : SV_Target {
 UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 half4 scr = tex2D(_MainTex, i.uv1);
 half4 ov = tex2D(_Overlay, i.uv0);
 return scr + _Intensity * ov;
}
half4 fragScreen(v2f_o i) : SV_Target {
 UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 half4 scr = tex2D(_MainTex, i.uv1);
 half4 ov = tex2D(_Overlay, i.uv0) * _Intensity;
 return 1.0 - (1.0 - saturate(scr)) * (1.0 - saturate(ov));
}
half4 fragMultiply(v2f_o i) : SV_Target {
 UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 half4 scr = tex2D(_MainTex, i.uv1);
 half4 ov = tex2D(_Overlay, i.uv0);
 return lerp(scr, scr * ov, saturate(_Intensity));
}
half4 fragOverlay(v2f_o i) : SV_Target {
 UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 half4 scr = tex2D(_MainTex, i.uv1);
 half4 ov = tex2D(_Overlay, i.uv0);
 half3 m = (scr.rgb < 0.5) ? (2.0 * scr.rgb * ov.rgb) : (1.0 - 2.0 * (1.0 - scr.rgb) * (1.0 - ov.rgb));
 return half4(lerp(scr.rgb, m, saturate(_Intensity)), scr.a);
}
half4 fragAlpha(v2f_o i) : SV_Target {
 UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 half4 scr = tex2D(_MainTex, i.uv1);
 half4 ov = tex2D(_Overlay, i.uv0);
 return lerp(scr, ov, saturate(_Intensity * ov.a));
}
ENDCG

SubShader {
 ZTest Always Cull Off ZWrite Off
 Pass { CGPROGRAM
#pragma vertex vertO
#pragma fragment fragAdditive
ENDCG }
 Pass { CGPROGRAM
#pragma vertex vertO
#pragma fragment fragScreen
ENDCG }
 Pass { CGPROGRAM
#pragma vertex vertO
#pragma fragment fragMultiply
ENDCG }
 Pass { CGPROGRAM
#pragma vertex vertO
#pragma fragment fragOverlay
ENDCG }
 Pass { CGPROGRAM
#pragma vertex vertO
#pragma fragment fragAlpha
ENDCG }
}
Fallback Off
}