Shader "Hidden/CC_BrightnessContrastGamma" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _rCoeff ("Luminance coeff (Red)", Range(0,1)) = 0.5
 _gCoeff ("Luminance coeff (Green)", Range(0,1)) = 0.5
 _bCoeff ("Luminance coeff (Blue)", Range(0,1)) = 0.5
 _brightness ("Brightness", Range(0,2)) = 1
 _contrast ("Contrast", Range(0,2)) = 1
 _gamma ("Gamma", Range(0.01,10)) = 1
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
  half _rCoeff;
  half _gCoeff;
  half _bCoeff;
  half _brightness;
  half _contrast;
  half _gamma;

  fixed4 frag(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
   half4 color = tex2D(_MainTex, i.uv);
   color.rgb *= _brightness;
   half3 coeff = half3(_rCoeff, _gCoeff, _bCoeff);
   color.rgb = (color.rgb - coeff) * _contrast + coeff;
   color.rgb = clamp(color.rgb, 0.0, 1.0);
   color.rgb = pow(max(color.rgb, 1e-5), _gamma);
   return color;
  }
  ENDCG
 }
}
Fallback Off
}