Shader "Hidden/BlendForBloom" {
Properties {
 _MainTex ("Screen Blended", 2D) = "" {}
 _ColorBuffer ("Color", 2D) = "" {}
}
CGINCLUDE
#include "UnityCG.cginc"
sampler2D _MainTex;
float4 _MainTex_TexelSize;
sampler2D _ColorBuffer;
float4 _ColorBuffer_TexelSize;
half _Intensity;

struct v2f_b {
 float4 pos : SV_POSITION;
 float2 uv0 : TEXCOORD0;
 float2 uv1 : TEXCOORD1;
 UNITY_VERTEX_OUTPUT_STEREO
};

v2f_b vertB(appdata_img v) {
 v2f_b o;
 UNITY_INITIALIZE_OUTPUT(v2f_b, o);
 UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
 o.pos = UnityObjectToClipPos(v.vertex);
 o.uv0 = v.texcoord.xy;
 o.uv1 = v.texcoord.xy;
 #if UNITY_UV_STARTS_AT_TOP
 if (_ColorBuffer_TexelSize.y < 0)
  o.uv1.y = 1.0 - o.uv1.y;
 #endif
 return o;
}

half4 fragScreen(v2f_b i) : SV_Target {
 UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 half4 addedbloom = tex2D(_MainTex, i.uv0) * _Intensity;
 half4 screencolor = tex2D(_ColorBuffer, i.uv1);
 return 1.0 - (1.0 - saturate(addedbloom)) * (1.0 - saturate(screencolor));
}

half4 fragAdd(v2f_b i) : SV_Target {
 UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 half4 addedbloom = tex2D(_MainTex, i.uv0) * _Intensity;
 half4 screencolor = tex2D(_ColorBuffer, i.uv1);
 return addedbloom + screencolor;
}

half4 fragDownsample(v2f_img i) : SV_Target {
 UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 float2 d = _MainTex_TexelSize.xy;
 half4 c = tex2D(_MainTex, i.uv + float2(-d.x, -d.y));
 c += tex2D(_MainTex, i.uv + float2( d.x, -d.y));
 c += tex2D(_MainTex, i.uv + float2(-d.x,  d.y));
 c += tex2D(_MainTex, i.uv + float2( d.x,  d.y));
 return c * 0.25;
}

half4 fragMultiply(v2f_b i) : SV_Target {
 UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 return tex2D(_MainTex, i.uv0) * tex2D(_ColorBuffer, i.uv1);
}

half4 fragCheapScreen(v2f_b i) : SV_Target {
 UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 half4 a = tex2D(_MainTex, i.uv0);
 half4 b = tex2D(_ColorBuffer, i.uv1);
 return 1.0 - (1.0 - saturate(a)) * (1.0 - saturate(b));
}

half4 fragCheapAdd(v2f_b i) : SV_Target {
 UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 return tex2D(_MainTex, i.uv0) + tex2D(_ColorBuffer, i.uv1);
}

half4 fragClearCopy(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 return tex2D(_MainTex, i.uv);
}

half4 fragVignetteInPlace(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 return tex2D(_ColorBuffer, i.uv);
}

half4 fragAddIntensity(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 return tex2D(_MainTex, i.uv) * _Intensity;
}
ENDCG

SubShader {
 ZTest Always Cull Off ZWrite Off
 // 0: Screen
 Pass {
  CGPROGRAM
#pragma vertex vertB
#pragma fragment fragScreen
ENDCG
 }
 // 1: Add
 Pass {
  CGPROGRAM
#pragma vertex vertB
#pragma fragment fragAdd
ENDCG
 }
 // 2: 4x Downsample
 Pass {
  CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragDownsample
ENDCG
 }
 // 3: Multiply Vignette
 Pass {
  CGPROGRAM
#pragma vertex vertB
#pragma fragment fragMultiply
ENDCG
 }
 // 4: Cheap Screen
 Pass {
  CGPROGRAM
#pragma vertex vertB
#pragma fragment fragCheapScreen
ENDCG
 }
 // 5: Cheap Add
 Pass {
  CGPROGRAM
#pragma vertex vertB
#pragma fragment fragCheapAdd
ENDCG
 }
 // 6: Copy
 Pass {
  CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragClearCopy
ENDCG
 }
 // 7: Multiply Vignette in-place
 Pass {
  Blend Zero SrcColor
  CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragVignetteInPlace
ENDCG
 }
 // 8: Blend One One copy
 Pass {
  Blend One One
  CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragClearCopy
ENDCG
 }
 // 9: AddTo with _Intensity
 Pass {
  Blend One One
  CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragAddIntensity
ENDCG
 }
 // 10: Max accumulate
 Pass {
  BlendOp Max
  Blend One One
  CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragClearCopy
ENDCG
 }
}
Fallback Off
}