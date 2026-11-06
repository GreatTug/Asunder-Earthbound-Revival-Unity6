Shader "Hidden/BrightPassFilter2" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "" {}
}
CGINCLUDE
#include "UnityCG.cginc"
sampler2D _MainTex;
half4 _Threshhold;
half4 threshhold;
half useSrcAlphaAsMask;

half4 fragScalar(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 half4 c = tex2D(_MainTex, i.uv);
 half t = (_Threshhold.x > 0.0) ? _Threshhold.x : threshhold.x;
 c.rgb *= lerp(1.0, c.a, saturate(useSrcAlphaAsMask));
 c.rgb = max(half3(0,0,0), c.rgb - t.xxx);
 return c;
}

half4 fragColor(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 half4 c = tex2D(_MainTex, i.uv);
 c.rgb = max(half3(0,0,0), c.rgb - _Threshhold.rgb);
 return c;
}
ENDCG

SubShader {
 ZTest Always Cull Off ZWrite Off
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragScalar
ENDCG }
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragColor
ENDCG }
}
Fallback Off
}