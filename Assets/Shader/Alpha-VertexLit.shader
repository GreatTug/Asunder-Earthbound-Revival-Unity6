Shader "Transparent/VertexLit" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _SpecColor ("Spec Color", Color) = (1,1,1,0)
 _Emission ("Emissive Color", Color) = (0,0,0,0)
 _Shininess ("Shininess", Range(0.1,1)) = 0.7
 _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}
SubShader {
 Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
 LOD 100
 CGPROGRAM
#pragma surface surf BlinnPhong alpha:fade
 sampler2D _MainTex;
 fixed4 _Color;
 fixed4 _Emission;
 half _Shininess;
 struct Input {
  float2 uv_MainTex;
 };
 void surf(Input IN, inout SurfaceOutput o) {
  fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
  o.Albedo = c.rgb * _Color.rgb;
  o.Emission = _Emission.rgb;
  o.Gloss = c.a;
  o.Specular = _Shininess;
  o.Alpha = c.a * _Color.a;
 }
 ENDCG
}
}