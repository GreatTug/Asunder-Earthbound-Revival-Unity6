Shader "Transparent/Cutout/Soft Edge Unlit" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
 _Cutoff ("Base Alpha cutoff", Range(0,0.9)) = 0.5
}
SubShader {
 Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout" }
 Lighting Off
 Cull Off
 Pass {
  CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog
  #include "UnityCG.cginc"

  struct appdata_t {
   float4 vertex : POSITION;
   fixed4 color : COLOR;
   float2 texcoord : TEXCOORD0;
   UNITY_VERTEX_INPUT_INSTANCE_ID
  };
  struct v2f {
   float4 vertex : SV_POSITION;
   fixed4 color : COLOR;
   float2 texcoord : TEXCOORD0;
   UNITY_FOG_COORDS(1)
   UNITY_VERTEX_OUTPUT_STEREO
  };
  sampler2D _MainTex;
  float4 _MainTex_ST;
  fixed4 _Color;
  fixed _Cutoff;

  v2f vert(appdata_t v) {
   v2f o;
   UNITY_SETUP_INSTANCE_ID(v);
   UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
   o.vertex = UnityObjectToClipPos(v.vertex);
   o.color = v.color;
   o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
   UNITY_TRANSFER_FOG(o, o.vertex);
   return o;
  }
  fixed4 frag(v2f i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
   fixed4 col = _Color * tex2D(_MainTex, i.texcoord);
   clip(col.a - _Cutoff);
   UNITY_APPLY_FOG(i.fogCoord, col);
   return col;
  }
  ENDCG
 }
 Pass {
  Tags { "RequireOption" = "SoftVegetation" }
  ZWrite Off
  Blend SrcAlpha OneMinusSrcAlpha
  CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog
  #include "UnityCG.cginc"

  struct appdata_t {
   float4 vertex : POSITION;
   fixed4 color : COLOR;
   float2 texcoord : TEXCOORD0;
   UNITY_VERTEX_INPUT_INSTANCE_ID
  };
  struct v2f {
   float4 vertex : SV_POSITION;
   fixed4 color : COLOR;
   float2 texcoord : TEXCOORD0;
   UNITY_FOG_COORDS(1)
   UNITY_VERTEX_OUTPUT_STEREO
  };
  sampler2D _MainTex;
  float4 _MainTex_ST;
  fixed4 _Color;
  fixed _Cutoff;

  v2f vert(appdata_t v) {
   v2f o;
   UNITY_SETUP_INSTANCE_ID(v);
   UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
   o.vertex = UnityObjectToClipPos(v.vertex);
   o.color = v.color;
   o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
   UNITY_TRANSFER_FOG(o, o.vertex);
   return o;
  }
  fixed4 frag(v2f i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
   fixed4 col = _Color * tex2D(_MainTex, i.texcoord);
   clip(_Cutoff - col.a);
   UNITY_APPLY_FOG(i.fogCoord, col);
   return col;
  }
  ENDCG
 }
}
}