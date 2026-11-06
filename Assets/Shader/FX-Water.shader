Shader "FX/Water" {
Properties {
 _WaveScale ("Wave scale", Range(0.02,0.15)) = 0.063
 _ReflDistort ("Reflection distort", Range(0,1.5)) = 0.44
 _RefrDistort ("Refraction distort", Range(0,1.5)) = 0.4
 _RefrColor ("Refraction color", Color) = (0.34,0.85,0.92,1)
 _Fresnel ("Fresnel (A) ", 2D) = "gray" {}
 _BumpMap ("Normalmap ", 2D) = "bump" {}
 WaveSpeed ("Wave speed (map1 x,y; map2 x,y)", Vector) = (19,9,-16,-7)
 _ReflectiveColor ("Reflective color (RGB) fresnel (A) ", 2D) = "white" {}
 _ReflectiveColorCube ("Reflective color cube (RGB) fresnel (A)", CUBE) = "" {}
 _HorizonColor ("Simple water horizon color", Color) = (0.172,0.463,0.435,1)
 _MainTex ("Fallback texture", 2D) = "white" {}
 _ReflectionTex ("Internal Reflection", 2D) = "white" {}
 _RefractionTex ("Internal Refraction", 2D) = "white" {}
}
SubShader {
 Tags { "WaterMode"="Refractive" "RenderType"="Opaque" }
 Pass {
  CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog
#pragma multi_compile WATER_REFRACTIVE WATER_REFLECTIVE WATER_SIMPLE
  #include "UnityCG.cginc"

  float4 _WaveScale4;
  float4 _WaveOffset;
  float _ReflDistort;
  float _RefrDistort;
  sampler2D _ReflectionTex;
  sampler2D _ReflectiveColor;
  sampler2D _RefractionTex;
  sampler2D _Fresnel;
  sampler2D _BumpMap;
  float4 _RefrColor;
  float4 _HorizonColor;

  struct appdata {
   float4 vertex : POSITION;
   float3 normal : NORMAL;
   UNITY_VERTEX_INPUT_INSTANCE_ID
  };

  struct v2f {
   float4 pos : SV_POSITION;
   float4 ref : TEXCOORD0;
   float2 bumpuv0 : TEXCOORD1;
   float2 bumpuv1 : TEXCOORD2;
   float3 viewDir : TEXCOORD3;
   UNITY_FOG_COORDS(4)
   UNITY_VERTEX_OUTPUT_STEREO
  };

  v2f vert(appdata v) {
   v2f o;
   UNITY_SETUP_INSTANCE_ID(v);
   UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
   o.pos = UnityObjectToClipPos(v.vertex);
   float4 temp;
   float4 wpos = mul(unity_ObjectToWorld, v.vertex);
   temp.xyzw = wpos.xzxz * _WaveScale4 + _WaveOffset;
   o.bumpuv0 = temp.xy;
   o.bumpuv1 = temp.wz;
   o.viewDir.xzy = WorldSpaceViewDir(v.vertex);
   o.ref = ComputeNonStereoScreenPos(o.pos);
   UNITY_TRANSFER_FOG(o, o.pos);
   return o;
  }

  half4 frag(v2f i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
   i.viewDir = normalize(i.viewDir);
   half3 bump1 = UnpackNormal(tex2D(_BumpMap, i.bumpuv0)).rgb;
   half3 bump2 = UnpackNormal(tex2D(_BumpMap, i.bumpuv1)).rgb;
   half3 bump = (bump1 + bump2) * 0.5;
   half fresnelFac = dot(i.viewDir, bump);

   half4 color;
   #if defined(WATER_REFRACTIVE)
    float4 uv1 = i.ref; uv1.xy += bump.xy * _ReflDistort;
    half4 refl = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(uv1));
    float4 uv2 = i.ref; uv2.xy -= bump.xy * _RefrDistort;
    half4 refr = tex2Dproj(_RefractionTex, UNITY_PROJ_COORD(uv2)) * _RefrColor;
    half fresnel = tex2D(_Fresnel, float2(fresnelFac, fresnelFac)).a;
    color = lerp(refr, refl, fresnel);
   #elif defined(WATER_REFLECTIVE)
    float4 uv1 = i.ref; uv1.xy += bump.xy * _ReflDistort;
    half4 refl = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(uv1));
    half4 water = tex2D(_ReflectiveColor, float2(fresnelFac, fresnelFac));
    color.rgb = lerp(water.rgb, refl.rgb, water.a);
    color.a = refl.a * water.a;
   #else
    half4 water = tex2D(_ReflectiveColor, float2(fresnelFac, fresnelFac));
    color.rgb = lerp(water.rgb, _HorizonColor.rgb, water.a);
    color.a = _HorizonColor.a;
   #endif

   UNITY_APPLY_FOG(i.fogCoord, color);
   return color;
  }
  ENDCG
 }
}
Fallback "Diffuse"
}