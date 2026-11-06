Shader "WFX/Additive (Soft) Alpha8" {
Properties {
 _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
 _MainTex ("Particle Texture (alpha)", 2D) = "white" {}
 _InvFade ("Soft Particles Factor", Range(0.01,3)) = 1
}
SubShader {
 Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
 Blend One OneMinusSrcColor
 Cull Off Lighting Off ZWrite Off
 Pass {
  CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_particles
#pragma multi_compile_fog
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  float4 _MainTex_ST;
  fixed4 _TintColor;
  UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
  float _InvFade;

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
   #ifdef SOFTPARTICLES_ON
   float4 projPos : TEXCOORD2;
   #endif
   UNITY_VERTEX_OUTPUT_STEREO
  };
  v2f vert(appdata_t v) {
   v2f o;
   UNITY_SETUP_INSTANCE_ID(v);
   UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
   o.vertex = UnityObjectToClipPos(v.vertex);
   #ifdef SOFTPARTICLES_ON
   o.projPos = ComputeScreenPos(o.vertex);
   COMPUTE_EYEDEPTH(o.projPos.z);
   #endif
   o.color = v.color;
   o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
   UNITY_TRANSFER_FOG(o, o.vertex);
   return o;
  }
  fixed4 frag(v2f i) : SV_Target {
   UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
   #ifdef SOFTPARTICLES_ON
   float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
   i.color.a *= saturate(_InvFade * (sceneZ - i.projPos.z));
   #endif
   fixed a = tex2D(_MainTex, i.uv).a;
   fixed4 col = 2.0f * i.color * _TintColor;
   col.a = saturate(col.a * a);
   col.rgb *= col.a;
   UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0));
   return col;
  }
  ENDCG
 }
}
}