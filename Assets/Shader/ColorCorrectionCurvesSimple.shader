Shader "Hidden/ColorCorrectionCurvesSimple" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "" {}
 _RgbTex ("_RgbTex (RGB)", 2D) = "" {}
}
SubShader {
 Pass {
  ZTest Always Cull Off ZWrite Off
  CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  sampler2D _RgbTex;
  half _Saturation;

  fixed4 frag(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
   half4 color = tex2D(_MainTex, i.uv);
   half3 ycoords = half3(0.125, 0.375, 0.625);
   half r = tex2D(_RgbTex, half2(color.r, ycoords.x)).r;
   half g = tex2D(_RgbTex, half2(color.g, ycoords.y)).g;
   half b = tex2D(_RgbTex, half2(color.b, ycoords.z)).b;
   half3 res = half3(r, g, b);
   half lum = Luminance(res);
   res = lerp(lum.xxx, res, _Saturation);
   return fixed4(res, color.a);
  }
  ENDCG
 }
}
Fallback Off
}