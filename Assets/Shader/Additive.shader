Shader "AngryBots/FX/Additive" {
Properties {
 _MainTex ("Base", 2D) = "white" {}
 _TintColor ("TintColor", Color) = (1,1,1,1)
}
SubShader {
 Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
 Cull Off
 Lighting Off
 ZWrite Off
 Blend SrcAlpha One
 Pass {
  CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog
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
   UNITY_FOG_COORDS(1)
   UNITY_VERTEX_OUTPUT_STEREO
  };

  v2f vert(appdata_t v) {
   v2f o;
   UNITY_SETUP_INSTANCE_ID(v);
   UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
   o.vertex = UnityObjectToClipPos(v.vertex);
   o.color = v.color;
   o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
   UNITY_TRANSFER_FOG(o, o.vertex);
   return o;
  }

  fixed4 frag(v2f i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
   fixed4 col = tex2D(_MainTex, i.uv) * _TintColor * i.color;
   UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0));
   return col;
  }
  ENDCG
 }
}
}