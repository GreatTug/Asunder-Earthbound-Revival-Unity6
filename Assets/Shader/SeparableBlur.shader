Shader "Hidden/SeparableBlur" {
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
  float4 offsets;

  half4 frag(v2f_img i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
   float2 d = offsets.xy;
   half4 color = 0.40 * tex2D(_MainTex, i.uv);
   color += 0.15 * tex2D(_MainTex, i.uv + d * 1.0);
   color += 0.15 * tex2D(_MainTex, i.uv - d * 1.0);
   color += 0.10 * tex2D(_MainTex, i.uv + d * 2.0);
   color += 0.10 * tex2D(_MainTex, i.uv - d * 2.0);
   color += 0.05 * tex2D(_MainTex, i.uv + d * 3.0);
   color += 0.05 * tex2D(_MainTex, i.uv - d * 3.0);
   return color;
  }
  ENDCG
 }
}
Fallback Off
}