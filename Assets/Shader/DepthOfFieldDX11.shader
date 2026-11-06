Shader "Hidden/Dof/DX11Dof" {
Properties {
 _MainTex ("", 2D) = "white" {}
 _BlurredColor ("", 2D) = "white" {}
 _FgCocMask ("", 2D) = "white" {}
}
SubShader {
 ZTest Always Cull Off ZWrite Off
 Pass {
  CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
  #include "UnityCG.cginc"
  sampler2D _MainTex;
  fixed4 frag(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i); return tex2D(_MainTex, i.uv); }
  ENDCG
 }
 Pass {
  Blend One One
  CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
  #include "UnityCG.cginc"
  fixed4 frag(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i); return fixed4(0,0,0,0); }
  ENDCG
 }
 Pass {
  Blend One One
  CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
  #include "UnityCG.cginc"
  fixed4 frag(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i); return fixed4(0,0,0,0); }
  ENDCG
 }
}
Fallback Off
}