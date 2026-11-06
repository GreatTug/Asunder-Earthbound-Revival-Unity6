Shader "Hidden/TerrainEngine/Details/BillboardWavingDoublePass" {
Properties {
 _WavingTint ("Fade Color", Color) = (0.7,0.6,0.5,0)
 _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
 _WaveAndDistance ("Wave and distance", Vector) = (12,3.6,1,1)
 _Cutoff ("Cutoff", Float) = 0.5
}
SubShader {
 Tags {
  "Queue" = "Geometry+200"
  "IgnoreProjector"="True"
  "RenderType"="GrassBillboard"
  "DisableBatching"="True"
 }
 Cull Off
 LOD 200
 CGPROGRAM
#pragma surface surf Lambert vertex:WavingGrassBillboardVert addshadow alphatest:_Cutoff
 #include "TerrainEngine.cginc"

 sampler2D _MainTex;

 struct Input {
  float2 uv_MainTex;
  fixed4 color : COLOR;
 };

 void surf(Input IN, inout SurfaceOutput o) {
  fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
  o.Albedo = c.rgb;
  o.Alpha = c.a;
 }
 ENDCG
}
Fallback Off
}