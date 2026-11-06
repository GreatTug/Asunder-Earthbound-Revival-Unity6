Shader "OVRGUIShader" {
Properties {
 _MainTex ("Texture", 2D) = "white" {}
}
SubShader {
 Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
 Lighting Off Cull Off ZTest Always ZWrite Off
 Blend SrcAlpha OneMinusSrcAlpha
 Pass {
  CGPROGRAM
#pragma vertex vert
#pragma fragment frag
  #include "UnityCG.cginc"
  sampler2D _MainTex;
  struct appdata_t {
   float4 vertex : POSITION;
   fixed4 color : COLOR;
   float2 uv : TEXCOORD0;
   UNITY_VERTEX_INPUT_INSTANCE_ID
  };
  struct v2f {
   float4 pos : SV_POSITION;
   fixed4 color : COLOR;
   float2 uv : TEXCOORD0;
   UNITY_VERTEX_OUTPUT_STEREO
  };
  v2f vert(appdata_t v) {
   v2f o;
   UNITY_SETUP_INSTANCE_ID(v);
   UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
   o.pos = UnityObjectToClipPos(v.vertex);
   o.color = v.color;
   o.uv = v.uv;
   return o;
  }
  fixed4 frag(v2f i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
   return tex2D(_MainTex, i.uv) * i.color;
  }
  ENDCG
 }
}
Fallback Off
}