Shader "Hidden/ContrastComposite" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "" {}
 _MainTexBlurred ("Base Blurred (RGB)", 2D) = "" {}
}
SubShader {
 Pass {
  ZTest Always Cull Off ZWrite Off
  CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  sampler2D _MainTexBlurred;
  half intensity;
  half threshhold;

  fixed4 frag(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
   half4 color = tex2D(_MainTex, i.uv);
   half4 blurred = tex2D(_MainTexBlurred, i.uv);
   half4 diff = color - blurred;
   half mask = saturate((length(diff.rgb) - threshhold) * 8.0);
   color.rgb += diff.rgb * intensity * mask;
   return color;
  }
  ENDCG
 }
}
Fallback Off
}