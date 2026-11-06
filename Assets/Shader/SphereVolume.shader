Shader "Advanced SS/Volumetric/Sphere Volume" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _RadiusSqr ("Radius", Float) = 1
 _TextureData ("Texture Speed (X,Y,Z), Texture Scale (W)", Vector) = (10,-0.1,0,1)
 _Visibility ("Visibility", Float) = 1
 _BIsForwardLighting ("Is Forward Lighting", Float) = 0
}
SubShader {
 Tags { "Queue"="Transparent+10" "IgnoreProjector"="True" "RenderType"="Transparent" }
 Pass {
  Cull Front
  ZWrite Off
  ZTest Always
  Blend SrcAlpha OneMinusSrcAlpha

  CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 3.0
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
  fixed4 _Color;
  float _RadiusSqr;
  float4 _TextureData;
  float _Visibility;

  struct appdata_v {
   float4 vertex : POSITION;
   UNITY_VERTEX_INPUT_INSTANCE_ID
  };

  struct v2f {
   float4 pos : SV_POSITION;
   float3 objPos : TEXCOORD0;
   float4 projPos : TEXCOORD1;
   float3 worldPos : TEXCOORD2;
   UNITY_VERTEX_OUTPUT_STEREO
  };

  v2f vert(appdata_v v) {
   v2f o;
   UNITY_SETUP_INSTANCE_ID(v);
   UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
   o.pos = UnityObjectToClipPos(v.vertex);
   o.objPos = v.vertex.xyz;
   o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
   o.projPos = ComputeScreenPos(o.pos);
   COMPUTE_EYEDEPTH(o.projPos.z);
   return o;
  }

  fixed4 frag(v2f i) : SV_Target {
   UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
   float3 camObj = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0)).xyz;
   float3 rayVec = i.objPos - camObj;
   float segLen = length(rayVec);
   float3 rayDir = rayVec / max(segLen, 1e-5);

   float b = dot(camObj, rayDir);
   float c = dot(camObj, camObj) - _RadiusSqr;
   float disc = b * b - c;
   if (disc <= 0.0) return fixed4(0,0,0,0);

   float sqrtDisc = sqrt(disc);
   float t0 = max(0.0, -b - sqrtDisc);
   float t1 = -b + sqrtDisc;

   float rawDepth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos));
   float sceneEyeDepth = LinearEyeDepth(rawDepth);
   float maxT = segLen * (sceneEyeDepth / max(i.projPos.z, 1e-4));
   t1 = min(t1, maxT);

   float dist = max(0.0, t1 - t0);
   if (dist <= 0.0) return fixed4(0,0,0,0);

   float midT = 0.5 * (t0 + t1);
   float3 midObj = camObj + rayDir * midT;
   float radialFade = saturate(1.0 - dot(midObj, midObj) / max(_RadiusSqr, 1e-4));

   float2 uv1 = (midObj.xz + midObj.y * 0.35) * _TextureData.w + _Time.y * _TextureData.xy;
   fixed4 noise = tex2D(_MainTex, uv1);

   float density = saturate(1.0 - exp(-dist / max(_Visibility, 0.05))) * radialFade;
   fixed4 col;
   col.rgb = _Color.rgb * lerp(1.0, noise.rgb, 0.5);
   col.a = saturate(density * _Color.a * lerp(1.0, noise.a, 0.35));
   return col;
  }
  ENDCG
 }
}
Fallback Off
}