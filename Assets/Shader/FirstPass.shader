Shader "Nature/Terrain/Diffuse" {
Properties {
 [HideInInspector] _Control ("Control (RGBA)", 2D) = "red" {}
 [HideInInspector] _Splat3 ("Layer 3 (A)", 2D) = "white" {}
 [HideInInspector] _Splat2 ("Layer 2 (B)", 2D) = "white" {}
 [HideInInspector] _Splat1 ("Layer 1 (G)", 2D) = "white" {}
 [HideInInspector] _Splat0 ("Layer 0 (R)", 2D) = "white" {}
 [HideInInspector] _MainTex ("BaseMap (RGB)", 2D) = "white" {}
 [HideInInspector] _Color ("Main Color", Color) = (1,1,1,1)
}
SubShader {
 Tags {
  "Queue" = "Geometry-100"
  "RenderType" = "Opaque"
 }
 CGPROGRAM
#pragma surface surf Lambert vertex:SplatmapVert finalcolor:SplatmapFinalColor
#pragma multi_compile_fog
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
Dependency "AddPassShader" = "Hidden/TerrainEngine/Splatmap/Lightmap-AddPass"
Dependency "Details0" = "Hidden/TerrainEngine/Details/Vertexlit"
Dependency "Details1" = "Hidden/TerrainEngine/Details/WavingDoublePass"
Dependency "Details2" = "Hidden/TerrainEngine/Details/BillboardWavingDoublePass"
Dependency "Tree0" = "Hidden/TerrainEngine/BillboardTree"
Fallback "Diffuse"
}