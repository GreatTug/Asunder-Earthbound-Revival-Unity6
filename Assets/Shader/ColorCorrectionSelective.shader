Shader "Hidden/ColorCorrectionSelective" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "" {}
}
SubShader {
 Pass {
  ZTest Always Cull Off ZWrite Off
  CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  fixed4 selColor;
  fixed4 targetColor;

  fixed4 frag(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
   fixed4 color = tex2D(_MainTex, i.uv);
   half diff = saturate(2.0 * length(color.rgb - selColor.rgb));
   color.rgb = lerp(targetColor.rgb, color.rgb, diff);
   return color;
  }
  ENDCG
 }
}
Fallback Off
}