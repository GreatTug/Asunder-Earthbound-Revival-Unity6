Shader "Hidden/CC_DoubleVision" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _displace ("Displace", Vector) = (0.7,0,0,0)
 _amount ("Amound", Float) = 1
}
SubShader {
 Pass {
  ZTest Always Cull Off ZWrite Off
  CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  float2 _displace;
  half _amount;

  fixed4 frag(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
   fixed4 c0 = tex2D(_MainTex, i.uv);
   fixed4 c1 = tex2D(_MainTex, i.uv + _displace);
   fixed4 c2 = tex2D(_MainTex, i.uv - _displace);
   return lerp(c0, (c1 + c2) * 0.5, saturate(_amount));
  }
  ENDCG
 }
}
Fallback Off
}