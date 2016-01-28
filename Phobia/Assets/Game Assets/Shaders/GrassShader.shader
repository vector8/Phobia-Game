Shader "Custom/GrassShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NormalTex ("Normals", 2D) = "bump" {}
		_OcclusionTex ("Ambient Occlusion", 2D) = "white" {}
		_Scale ("Multi UV Scale", Range(0.001, 1.0)) = 0.25
		_Scale2 ("Multi UV Scale 2", Range(0.001, 1.0)) = 0.0625
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _NormalTex;
		sampler2D _OcclusionTex;

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;
		float _Scale;
		float _Scale2;

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * tex2D (_OcclusionTex, IN.uv_MainTex) * 0.3333;
			c += tex2D (_MainTex, IN.uv_MainTex * _Scale) * tex2D (_OcclusionTex, IN.uv_MainTex * _Scale) * 0.3333;
			c += tex2D (_MainTex, IN.uv_MainTex * _Scale2) * tex2D (_OcclusionTex, IN.uv_MainTex * _Scale2) * 0.3333;
			o.Albedo = c.rgb * _Color.rgb;
			// Metallic and smoothness come from slider variables
			o.Normal = UnpackNormal(tex2D(_NormalTex, IN.uv_MainTex));
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
