Shader "Hidden/BlurEffectConeTap" {
Properties {
 _MainTex ("", 2D) = "" {}
}
SubShader {
 Pass {
  ZTest Always Cull Off ZWrite Off
  CGPROGRAM
#pragma vertex vert
#pragma fragment frag
  #include "UnityCG.cginc"

  struct v2f {
   float4 pos : SV_POSITION;
   half2 uv : TEXCOORD0;
   half2 taps[4] : TEXCOORD1;
   UNITY_VERTEX_OUTPUT_STEREO
  };

  sampler2D _MainTex;
  half4 _MainTex_TexelSize;
  half4 _BlurOffsets;

  v2f vert(appdata_img v) {
   v2f o;
   UNITY_INITIALIZE_OUTPUT(v2f, o);
   UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
   o.pos = UnityObjectToClipPos(v.vertex);
   o.uv = v.texcoord - _BlurOffsets.xy * _MainTex_TexelSize.xy;
   o.taps[0] = o.uv + _MainTex_TexelSize.xy * _BlurOffsets.xy;
   o.taps[1] = o.uv - _MainTex_TexelSize.xy * _BlurOffsets.xy;
   o.taps[2] = o.uv + _MainTex_TexelSize.xy * _BlurOffsets.xy * half2(1, -1);
   o.taps[3] = o.uv - _MainTex_TexelSize.xy * _BlurOffsets.xy * half2(1, -1);
   return o;
  }

  fixed4 frag(v2f i) : SV_Target {
   UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
   half4 color = tex2D(_MainTex, i.taps[0]);
   color += tex2D(_MainTex, i.taps[1]);
   color += tex2D(_MainTex, i.taps[2]);
   color += tex2D(_MainTex, i.taps[3]);
   return color * 0.25;
  }
  ENDCG
 }
}
Fallback Off
}