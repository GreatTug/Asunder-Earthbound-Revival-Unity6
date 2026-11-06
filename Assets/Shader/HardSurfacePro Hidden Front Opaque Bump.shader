Shader "Hidden/Hardsurface Pro Front Opaque Bump" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Color ("Color", Color) = (1,1,1,1)
}
SubShader {
 Tags { "RenderType"="Opaque" }
 Pass {
  Name "FORWARD"
  Tags { "LightMode"="ForwardBase" }
  CGPROGRAM
#pragma vertex vert
#pragma fragment frag
  #include "UnityCG.cginc"
  sampler2D _MainTex;
  fixed4 _Color;
  struct v2f {
   float4 pos : SV_POSITION;
   float2 uv : TEXCOORD0;
   UNITY_VERTEX_OUTPUT_STEREO
  };
  v2f vert(appdata_base v) {
   v2f o;
   UNITY_SETUP_INSTANCE_ID(v);
   UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
   o.pos = UnityObjectToClipPos(v.vertex);
   o.uv = v.texcoord.xy;
   return o;
  }
  fixed4 frag(v2f i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
   return tex2D(_MainTex, i.uv) * _Color;
  }
  ENDCG
 }
}
}