Shader "Hidden/GlobalFog" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader {
	ZTest Always Cull Off ZWrite Off

	CGINCLUDE
	#include "UnityCG.cginc"

	sampler2D _MainTex;
	UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

	float4x4 _FrustumCornersWS;
	float4 _CameraWS;
	float4 _StartDistance;
	float4 _Y;
	float _GlobalDensity;
	fixed4 _FogColor;

	struct appdata {
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float3 interpolatedRay : TEXCOORD1;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	v2f vert(appdata v) {
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		o.pos = float4(v.vertex.x * 2.0 - 1.0, v.vertex.y * 2.0 - 1.0, 0.0, 1.0);
		o.uv = v.texcoord;
		int index = (int)v.vertex.z;
		if (index == 0) o.interpolatedRay = _FrustumCornersWS[0].xyz;
		else if (index == 1) o.interpolatedRay = _FrustumCornersWS[1].xyz;
		else if (index == 2) o.interpolatedRay = _FrustumCornersWS[2].xyz;
		else o.interpolatedRay = _FrustumCornersWS[3].xyz;
		return o;
	}

	float ComputeHeightFog(float wsY) {
		float h = (wsY - _Y.x) * _Y.y;
		return saturate(exp(-h * h));
	}

	float ComputeDistanceFog(float dist) {
		float d = max(dist - _StartDistance.y, 0.0) * _GlobalDensity;
		return saturate(1.0 - exp(-d));
	}

	fixed4 fragAbsYAndDist(v2f i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
		fixed4 original = tex2D(_MainTex, i.uv);
		float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
		float linearDepth = Linear01Depth(depth);
		if (linearDepth >= 0.9999) return original;
		float3 wsPos = _CameraWS.xyz + i.interpolatedRay * linearDepth;
		float dist = length(i.interpolatedRay * linearDepth);
		float fogFactor = saturate(ComputeDistanceFog(dist) * ComputeHeightFog(wsPos.y));
		return lerp(original, _FogColor, fogFactor);
	}

	fixed4 fragAbsY(v2f i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
		fixed4 original = tex2D(_MainTex, i.uv);
		float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
		float linearDepth = Linear01Depth(depth);
		if (linearDepth >= 0.9999) return original;
		float3 wsPos = _CameraWS.xyz + i.interpolatedRay * linearDepth;
		float fogFactor = saturate(ComputeHeightFog(wsPos.y) * _GlobalDensity * 10.0);
		return lerp(original, _FogColor, fogFactor);
	}

	fixed4 fragDist(v2f i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
		fixed4 original = tex2D(_MainTex, i.uv);
		float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
		float linearDepth = Linear01Depth(depth);
		if (linearDepth >= 0.9999) return original;
		float dist = length(i.interpolatedRay * linearDepth);
		float fogFactor = saturate(ComputeDistanceFog(dist));
		return lerp(original, _FogColor, fogFactor);
	}

	fixed4 fragRelYAndDist(v2f i) : SV_Target {
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
		fixed4 original = tex2D(_MainTex, i.uv);
		float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
		float linearDepth = Linear01Depth(depth);
		if (linearDepth >= 0.9999) return original;
		float3 wsPos = _CameraWS.xyz + i.interpolatedRay * linearDepth;
		float dist = length(i.interpolatedRay * linearDepth);
		float relY = wsPos.y - _CameraWS.y;
		float fogFactor = saturate(ComputeDistanceFog(dist) * ComputeHeightFog(relY));
		return lerp(original, _FogColor, fogFactor);
	}
	ENDCG

	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment fragAbsYAndDist
		ENDCG
	}
	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment fragAbsY
		ENDCG
	}
	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment fragDist
		ENDCG
	}
	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment fragRelYAndDist
		ENDCG
	}
}
Fallback Off
}
