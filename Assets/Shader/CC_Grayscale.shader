Shader "Hidden/CC_Grayscale" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _rLum ("Luminance (Red)", Range(0,1)) = 0.3
 _gLum ("Luminance (Green)", Range(0,1)) = 0.59
 _bLum ("Luminance (Blue)", Range(0,1)) = 0.11
 _amount ("Amount", Range(0,1)) = 1
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
  half _rLum;
  half _gLum;
  half _bLum;
  half _amount;

  fixed4 frag(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
   half4 color = tex2D(_MainTex, i.uv);
   half lum = dot(color.rgb, half3(_rLum, _gLum, _bLum));
   color.rgb = lerp(color.rgb, lum.xxx, saturate(_amount));
   return color;
  }
  ENDCG
 }
}
Fallback Off
}