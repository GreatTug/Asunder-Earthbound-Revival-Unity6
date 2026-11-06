Shader "OVRLensCorrection" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader {
 Pass {
  ZTest Always Cull Off ZWrite Off
  CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
  #include "UnityCG.cginc"
  sampler2D _MainTex;
  fixed4 frag(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
   return tex2D(_MainTex, i.uv);
  }
  ENDCG
 }
}
Fallback Off
}