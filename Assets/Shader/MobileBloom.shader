Shader "Hidden/FastBloom" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Bloom ("Bloom (RGB)", 2D) = "black" {}
}
CGINCLUDE
#include "UnityCG.cginc"
sampler2D _MainTex;
sampler2D _Bloom;
half4 _MainTex_TexelSize;
half4 _Parameter;

half4 fragBloomComposite(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 half4 baseCol = tex2D(_MainTex, i.uv);
 half4 bloomCol = tex2D(_Bloom, i.uv);
 return baseCol + bloomCol * _Parameter.w;
}

half4 fragDownsampleThreshold(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 float2 d = _MainTex_TexelSize.xy;
 half4 c = tex2D(_MainTex, i.uv + float2(-d.x, -d.y));
 c += tex2D(_MainTex, i.uv + float2( d.x, -d.y));
 c += tex2D(_MainTex, i.uv + float2(-d.x,  d.y));
 c += tex2D(_MainTex, i.uv + float2( d.x,  d.y));
 c *= 0.25;
 c.rgb = max(half3(0,0,0), c.rgb - _Parameter.zzz);
 return c;
}

half4 fragBlurVert(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 float2 d = float2(0.0, _MainTex_TexelSize.y * _Parameter.x);
 half4 c = tex2D(_MainTex, i.uv) * 0.227027027;
 c += tex2D(_MainTex, i.uv + d * 1.38461538) * 0.316216216;
 c += tex2D(_MainTex, i.uv - d * 1.38461538) * 0.316216216;
 c += tex2D(_MainTex, i.uv + d * 3.23076923) * 0.070270270;
 c += tex2D(_MainTex, i.uv - d * 3.23076923) * 0.070270270;
 return c;
}

half4 fragBlurHoriz(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 float2 d = float2(_MainTex_TexelSize.x * _Parameter.x, 0.0);
 half4 c = tex2D(_MainTex, i.uv) * 0.227027027;
 c += tex2D(_MainTex, i.uv + d * 1.38461538) * 0.316216216;
 c += tex2D(_MainTex, i.uv - d * 1.38461538) * 0.316216216;
 c += tex2D(_MainTex, i.uv + d * 3.23076923) * 0.070270270;
 c += tex2D(_MainTex, i.uv - d * 3.23076923) * 0.070270270;
 return c;
}
ENDCG

SubShader {
 ZTest Always Cull Off ZWrite Off
 // 0: Composite
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragBloomComposite
ENDCG }
 // 1: Downsample + Threshold
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragDownsampleThreshold
ENDCG }
 // 2: Vertical Blur (Standard)
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragBlurVert
ENDCG }
 // 3: Horizontal Blur (Standard)
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragBlurHoriz
ENDCG }
 // 4: Vertical Blur (SGX)
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragBlurVert
ENDCG }
 // 5: Horizontal Blur (SGX)
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragBlurHoriz
ENDCG }
}
Fallback Off
}