Shader "Custom/Stars" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		[HideInInspector] _MainTex("_MainTex", 2D) = "white"

		iterations ("Iterations", Int) = 17
		formuparam ("FormUParam", Float) = 0.53

		volsteps ("VolSteps", Int) = 20
		stepsize ("StepSize", Float) = 0.1

		zoom  ("Zoom", Float) = 0.800
		tile  ("Tile", Float) = 0.850
		speed ("Speed", Float) = 0.010 

		brightness ("Brightness", Float) = 0.0015
		darkmatter ("DarkMatter", Float) = 0.300	
		distfading ("DistFading", Float) = 0.730	
		saturation ("Saturation", Float) = 0.850	
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Lighting Off
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;
		int iterations;
		float formuparam;

		int volsteps;
		float stepsize;

		float zoom;
		float tile;
		float speed;

		float brightness;
		float darkmatter;
		float distfading;
		float saturation;

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			// Star Nest by Pablo Román Andrioli

			// Get coords and direction
			float2 uv = IN.uv_MainTex;

			float3 dir = float3(uv.x * zoom, uv.y * zoom, 1.0);
			float time = _Time.y * speed + 0.25;

			float3 from = float3(1.0, 0.5, 0.5);
			from += float3(time * 2.0, time, -2.0);
						
			// Volumetric rendering
			float s = 0.1, fade = 1.0;

			float3 v = float3(0.0, 0.0, 0.0);

			for (int r = 0; r < volsteps; r++)
			{
				float3 p = from + s * dir * 0.5;
				p = abs(float3(tile, tile, tile) - fmod(p, float3(tile * 2.0, tile * 2.0, tile * 2.0))); // Tiling fold
				float pa, a = pa = 0.0;
				for (int i = 0; i < iterations; i++)
				{ 
					p = abs(p) / dot(p, p) - formuparam; // The magic formula
					a += abs(length(p) - pa); // Absolute sum of average change
					pa = length(p);
				}
				float dm = max(0.0, darkmatter - a * a * 0.001); // Dark matter
				a *= a * a; // Add contrast
				if (r > 6) 
				{
					fade *= 1.0 - dm; // Dark matter, don't render near
				}
				// v += float3(dm, dm * 0.5, 0.0);
				v += fade;
				v += float3(s, s * s, s * s * s * s) * a * brightness * fade; // Coloring based on distance
				fade *= distfading; // Distance fading
				s += stepsize;
			}

			v = lerp(float3(length(v), length(v), length(v)), v, saturation); // Color adjust
			o.Albedo = (v * 0.01) * _Color.rgb;	
			o.Alpha = _Color.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
