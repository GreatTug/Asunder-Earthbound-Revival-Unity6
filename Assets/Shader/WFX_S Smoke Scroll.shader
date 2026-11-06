Shader "WFX/Scroll/Smoke" {
Properties {
 _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
 _MainTex ("Texture", 2D) = "white" {}
 _ScrollSpeed ("Scroll Speed", Float) = 2
}
SubShader {
 Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
 Blend SrcAlpha OneMinusSrcAlpha
 Cull Off Lighting Off ZWrite Off
 Pass {
  CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  float4 _MainTex_ST;
  fixed4 _TintColor;
  float _ScrollSpeed;

  struct appdata_t {
   float4 vertex : POSITION;
   fixed4 color : COLOR;
   float2 texcoord : TEXCOORD0;
   UNITY_VERTEX_INPUT_INSTANCE_ID
  };
  struct v2f {
   float4 vertex : SV_POSITION;
   fixed4 color : COLOR;
   float2 uv : TEXCOORD0;
   UNITY_FOG_COORDS(1)
   UNITY_VERTEX_OUTPUT_STEREO
  };
  v2f vert(appdata_t v) {
   v2f o;
   UNITY_SETUP_INSTANCE_ID(v);
   UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
   o.vertex = UnityObjectToClipPos(v.vertex);
   o.color = v.color;
   o.uv = TRANSFORM_TEX(v.texcoord, _MainTex) + float2(0.0, _ScrollSpeed * _Time.y * 0.1);
   UNITY_TRANSFER_FOG(o, o.vertex);
   return o;
  }
  fixed4 frag(v2f i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
   fixed4 col = 2.0f * i.color * _TintColor * tex2D(_MainTex, i.uv);
   col.a = saturate(col.a);
   UNITY_APPLY_FOG(i.fogCoord, col);
   return col;
  }
  ENDCG
 }
}
}