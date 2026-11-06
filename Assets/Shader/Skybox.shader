Shader "RenderFX/Skybox" {
Properties {
 _Tint ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
 [NoScaleOffset] _FrontTex ("Front (+Z)", 2D) = "white" {}
 [NoScaleOffset] _BackTex ("Back (-Z)", 2D) = "white" {}
 [NoScaleOffset] _LeftTex ("Left (+X)", 2D) = "white" {}
 [NoScaleOffset] _RightTex ("Right (-X)", 2D) = "white" {}
 [NoScaleOffset] _UpTex ("Up (+Y)", 2D) = "white" {}
 [NoScaleOffset] _DownTex ("down (-Y)", 2D) = "white" {}
}
SubShader {
 Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
 Cull Off ZWrite Off

 CGINCLUDE
 #include "UnityCG.cginc"
 fixed4 _Tint;
 struct appdata_t {
  float4 vertex : POSITION;
  float2 texcoord : TEXCOORD0;
  UNITY_VERTEX_INPUT_INSTANCE_ID
 };
 struct v2f {
  float4 vertex : SV_POSITION;
  float2 texcoord : TEXCOORD0;
  UNITY_VERTEX_OUTPUT_STEREO
 };
 v2f vert(appdata_t v) {
  v2f o;
  UNITY_SETUP_INSTANCE_ID(v);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  o.vertex = UnityObjectToClipPos(v.vertex);
  o.texcoord = v.texcoord;
  return o;
 }
 half4 skybox_frag(v2f i, sampler2D smp) {
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
  half4 tex = tex2D(smp, i.texcoord);
  half4 c;
  c.rgb = tex.rgb * _Tint.rgb * unity_ColorSpaceDouble.rgb;
  c.a = tex.a * _Tint.a * 2.0;
  return c;
 }
 ENDCG

 Pass {
  CGPROGRAM
#pragma vertex vert
#pragma fragment frag
  sampler2D _FrontTex;
  half4 frag(v2f i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i); return skybox_frag(i, _FrontTex); }
  ENDCG
 }
 Pass {
  CGPROGRAM
#pragma vertex vert
#pragma fragment frag
  sampler2D _BackTex;
  half4 frag(v2f i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i); return skybox_frag(i, _BackTex); }
  ENDCG
 }
 Pass {
  CGPROGRAM
#pragma vertex vert
#pragma fragment frag
  sampler2D _LeftTex;
  half4 frag(v2f i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i); return skybox_frag(i, _LeftTex); }
  ENDCG
 }
 Pass {
  CGPROGRAM
#pragma vertex vert
#pragma fragment frag
  sampler2D _RightTex;
  half4 frag(v2f i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i); return skybox_frag(i, _RightTex); }
  ENDCG
 }
 Pass {
  CGPROGRAM
#pragma vertex vert
#pragma fragment frag
  sampler2D _UpTex;
  half4 frag(v2f i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i); return skybox_frag(i, _UpTex); }
  ENDCG
 }
 Pass {
  CGPROGRAM
#pragma vertex vert
#pragma fragment frag
  sampler2D _DownTex;
  half4 frag(v2f i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i); return skybox_frag(i, _DownTex); }
  ENDCG
 }
}
}