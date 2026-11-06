Shader "Hidden/SimpleClear" {
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
  fixed4 frag(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
   return fixed4(0,0,0,0);
  }
  ENDCG
 }
}
Fallback Off
}