Shader "Hidden/Dof/DepthOfFieldHdr" {
Properties {
 _MainTex ("-", 2D) = "black" {}
 _FgOverlap ("-", 2D) = "black" {}
 _LowRez ("-", 2D) = "black" {}
}
CGINCLUDE
#include "UnityCG.cginc"
sampler2D _MainTex;
float4 _MainTex_TexelSize;
sampler2D _FgOverlap;
sampler2D _LowRez;
UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
float4 _CurveParams;
float4 _Offsets;

float CalcCoc(float2 uv) {
 float d = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv));
 float coc = abs(d - _CurveParams.w) * _CurveParams.z - _CurveParams.y;
 return saturate(coc);
}

half4 fragWriteCoc(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 half4 c = tex2D(_MainTex, i.uv);
 c.a = CalcCoc(i.uv);
 return c;
}

half4 fragCopy(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 return tex2D(_MainTex, i.uv);
}

half4 fragDilateBlur(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 float2 d = _Offsets.xy * _MainTex_TexelSize.xy;
 half4 c = tex2D(_MainTex, i.uv) * 0.4;
 c += tex2D(_MainTex, i.uv + d) * 0.3;
 c += tex2D(_MainTex, i.uv - d) * 0.3;
 return c;
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

half4 fragDiscBlur(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 half4 center = tex2D(_MainTex, i.uv);
 float radius = center.a * _Offsets.w;
 float2 px = _MainTex_TexelSize.xy * radius;
 half4 sum = center;
 sum += tex2D(_MainTex, i.uv + px * float2( 1.0,  0.0));
 sum += tex2D(_MainTex, i.uv + px * float2(-1.0,  0.0));
 sum += tex2D(_MainTex, i.uv + px * float2( 0.0,  1.0));
 sum += tex2D(_MainTex, i.uv + px * float2( 0.0, -1.0));
 sum += tex2D(_MainTex, i.uv + px * float2( 0.7,  0.7));
 sum += tex2D(_MainTex, i.uv + px * float2(-0.7,  0.7));
 sum += tex2D(_MainTex, i.uv + px * float2( 0.7, -0.7));
 sum += tex2D(_MainTex, i.uv + px * float2(-0.7, -0.7));
 return sum / 9.0;
}

half4 fragUpsampleComposite(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 half4 hi = tex2D(_MainTex, i.uv);
 half4 lo = tex2D(_LowRez, i.uv);
 return lerp(hi, lo, saturate(hi.a));
}

half4 fragVisualize(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 float coc = CalcCoc(i.uv);
 return half4(coc.xxx, 1.0);
}
ENDCG

SubShader {
 ZTest Always Cull Off ZWrite Off
 // 0: WriteCoc
 Pass { ColorMask A CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragWriteCoc
ENDCG }
 // 1
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragCopy
ENDCG }
 // 2:FgBlur
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragDilateBlur
ENDCG }
 // 3
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragCopy
ENDCG }
 // 4:CaptureFg
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragWriteCoc
ENDCG }
 // 5:BoxBlur
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragDilateBlur
ENDCG }
 // 6:Downsample
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragDownsample
ENDCG }
 // 7
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragCopy
ENDCG }
 // 8
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragCopy
ENDCG }
 // 9:DX11 Upsample
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragUpsampleComposite
ENDCG }
 // 10
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragCopy
ENDCG }
 // 11:DiscBlur Low
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragDiscBlur
ENDCG }
 // 12:Upsample Med
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragUpsampleComposite
ENDCG }
 // 13:MergeCoc
 Pass { ColorMask A CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragWriteCoc
ENDCG }
 // 14
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragCopy
ENDCG }
 // 15:Downsample2
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragDownsample
ENDCG }
 // 16:Visualize
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragVisualize
ENDCG }
 // 17:DiscBlur High
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragDiscBlur
ENDCG }
 // 18:Upsample High
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragUpsampleComposite
ENDCG }
 // 19:Blur
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragDilateBlur
ENDCG }
 // 20
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragCopy
ENDCG }
 // 21
 Pass { CGPROGRAM
#pragma vertex vert_img
#pragma fragment fragDilateBlur
ENDCG }
}
Fallback Off
}