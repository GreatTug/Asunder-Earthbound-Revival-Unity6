Shader "WFX/Multiply Soft Tint" {
Properties {
 _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
 _MainTex ("Texture", 2D) = "white" {}
}
SubShader {
 Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
 Blend Zero SrcColor
 Cull Off Lighting Off ZWrite Off
 Pass {
  CGPROGRAM
#pragma vertex vert
#pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  float4 _MainTex_ST;
  fixed4 _TintColor;

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
   UNITY_VERTEX_OUTPUT_STEREO
  };
  v2f vert(appdata_t v) {
   v2f o;
   UNITY_SETUP_INSTANCE_ID(v);
   UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
   o.vertex = UnityObjectToClipPos(v.vertex);
   o.color = v.color;
   o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
   return o;
  }
  fixed4 frag(v2f i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
   fixed4 tex = tex2D(_MainTex, i.uv);
   fixed4 prod = tex * _TintColor * i.color * 2.0;
   return lerp(fixed4(1,1,1,1), prod, saturate(tex.a * i.color.a * _TintColor.a * 2.0));
  }
  ENDCG
 }
}
}