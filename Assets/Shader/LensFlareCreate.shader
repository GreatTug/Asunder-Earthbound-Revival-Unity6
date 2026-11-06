Shader "Hidden/LensFlareCreate" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "" {}
}
SubShader {
 Pass {
  ZTest Always Cull Off ZWrite Off
  Blend One One
  CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  half4 colorA;
  half4 colorB;
  half4 colorC;
  half4 colorD;

  half4 frag(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
   float2 c = i.uv - 0.5;
   half4 col = tex2D(_MainTex, c * -0.85 + 0.5) * colorA;
   col += tex2D(_MainTex, c * -1.45 + 0.5) * colorB;
   col += tex2D(_MainTex, c * -2.55 + 0.5) * colorC;
   col += tex2D(_MainTex, c * -4.15 + 0.5) * colorD;
   return col;
  }
  ENDCG
 }
}
Fallback Off
}