Shader "Hidden/SunShaftsComposite" {
Properties {
 _MainTex ("Base", 2D) = "" {}
 _ColorBuffer ("Color", 2D) = "" {}
 _Skybox ("Skybox", 2D) = "" {}
}
CGINCLUDE
#include "UnityCG.cginc"
sampler2D _MainTex;
sampler2D _ColorBuffer;
sampler2D _Skybox;
UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
half4 _SunColor;
half4 _BlurRadius4;
half4 _SunPosition;
half _NoSkyBoxMask;

half4 fragScreen(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 half4 colorA = tex2D(_MainTex, i.uv);
 half4 colorB = saturate(tex2D(_ColorBuffer, i.uv) * _SunColor);
 return 1.0 - (1.0 - colorA) * (1.0 - colorB);
}

half4 fragRadialBlur(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 float2 delta = (_SunPosition.xy - i.uv) * _BlurRadius4.xy;
 half4 col = half4(0,0,0,0);
 float2 uv = i.uv;
 for (int j = 0; j < 6; j++) {
  col += tex2D(_MainTex, uv);
  uv += delta;
 }
 return col / 6.0;
}

half4 fragDepthMask(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 float depthSample = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv));
 half4 tex = tex2D(_MainTex, i.uv);
 half dist = saturate(_SunPosition.w - length(_SunPosition.xy - i.uv));
 half lum = max(dot(tex.rgb, half3(0.59, 0.3, 0.11)) - 0.05, 0.0);
 return (depthSample > 0.99) ? half4((lum * dist).xxx, 1.0) : half4(0,0,0,0);
}

half4 fragNoDepthMask(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 half4 sky = tex2D(_Skybox, i.uv);
 half4 tex = tex2D(_MainTex, i.uv);
 half dist = saturate(_SunPosition.w - length(_SunPosition.xy - i.uv));
 half diff = (abs(sky.r - tex.r) + abs(sky.g - tex.g) + abs(sky.b - tex.b)) < 0.2 ? 1.0 : 0.0;
 half lum = max(dot(sky.rgb, half3(0.59, 0.3, 0.11)), 0.0);
 return half4((lum * dist * diff).xxx, 1.0);
}

half4 fragAdd(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 return tex2D(_MainTex, i.uv) + saturate(tex2D(_ColorBuffer, i.uv) * _SunColor);
}
ENDCG

SubShader {
 ZTest Always Cull Off ZWrite Off
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragScreen
ENDCG }
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragRadialBlur
ENDCG }
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragDepthMask
ENDCG }
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragNoDepthMask
ENDCG }
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragAdd
ENDCG }
}
Fallback Off
}