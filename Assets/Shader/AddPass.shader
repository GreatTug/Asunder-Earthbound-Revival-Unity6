Shader "Hidden/TerrainEngine/Splatmap/Lightmap-AddPass" {
Properties {
 _Control ("Control (RGBA)", 2D) = "black" {}
 _Splat3 ("Layer 3 (A)", 2D) = "white" {}
 _Splat2 ("Layer 2 (B)", 2D) = "white" {}
 _Splat1 ("Layer 1 (G)", 2D) = "white" {}
 _Splat0 ("Layer 0 (R)", 2D) = "white" {}
}
SubShader {
 Tags {
  "Queue" = "Geometry-99"
  "IgnoreProjector"="True"
  "RenderType" = "Opaque"
 }
 CGPROGRAM
#pragma surface surf Lambert decal:add vertex:SplatmapVert finalcolor:SplatmapFinalColor
#pragma multi_compile_fog
 #define TERRAIN_SPLAT_ADDPASS
 #include "TerrainSplatmapCommon.cginc"

 void surf(Input IN, inout SurfaceOutput o) {
  half4 splat_control;
  half weight;
  fixed4 mixedDiffuse;
  SplatmapMix(IN, splat_control, weight, mixedDiffuse, o.Normal);
  o.Albedo = mixedDiffuse.rgb;
  o.Alpha = weight;
 }
 ENDCG
}
Fallback Off
}