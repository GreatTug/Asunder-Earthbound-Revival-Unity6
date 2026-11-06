Shader "Hidden/CC_FastVignette" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _sharpness ("Sharpness", Range(-1,1)) = 0.1
 _darkness ("Darkness", Range(0,2)) = 0.3
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
  half _sharpness;
  half _darkness;

  fixed4 frag(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
   half4 color = tex2D(_MainTex, i.uv);
   half dist = distance(i.uv, half2(0.5, 0.5));
   half v = (_darkness <= 0.0001) ? 1.0 : smoothstep(0.8, _sharpness * 0.799, dist * (_darkness + _sharpness));
   color.rgb *= saturate(v);
   return color;
  }
  ENDCG
 }
}
Fallback Off
}