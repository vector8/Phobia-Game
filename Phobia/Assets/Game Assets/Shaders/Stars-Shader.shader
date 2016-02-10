Shader "Davids Gifts/Stars"
{
	Properties
	{
		_Color ("B&W - Color", Range(0, 1)) = 1
		[HideInInspector] _HueShift ("Hue", Range(0, 1.0)) = 0
		[HideInInspector] _Saturation ("Saturation", Range(0, 5.0)) = 1
		[HideInInspector] _Lightness ("Value", Range(0, 5.0)) = 1

		[HideInInspector] _MainTex("_MainTex", 2D) = "white"

		[HideInInspector] iterations ("Iterations", Int) = 17
		[HideInInspector] formuparam ("FormUParam", Float) = 0.53

		[HideInInspector] volsteps ("VolSteps", Int) = 20
		[HideInInspector] stepsize ("StepSize", Float) = 0.1

		[HideInInspector] zoom  ("Zoom", Float) = 0.800
		[HideInInspector] tile  ("Tile", Float) = 0.850
		[HideInInspector] speed ("Speed", Float) = 0.010 

		[HideInInspector] brightness ("Brightness", Float) = 0.0015
		[HideInInspector] darkmatter ("DarkMatter", Float) = 0.300	
		[HideInInspector] distfading ("DistFading", Float) = 0.730	
		[HideInInspector] saturation ("Saturation", Float) = 0.850	

		startstep ("StartStep", Int) = 0
		position2D ("2D Position/Z-Rotation/Speed Multiplier", Vector) = (0,0,0,0.1)
		_AspectRatio ("Aspect Ratio", Float) = 1.0
		[KeywordEnum(SrcColor, SrcAlpha, One, OneMinusDstColor, DstColor)] _OverlaySrc ("SrcFactor", Int) = 3
		[KeywordEnum(Off, OneMinusSrcAlpha, OneMinusSrcAlpha, One, Zero, SrcColor)] _OverlayDst ("DstFactor", Int) = 7
		[KeywordEnum(Less, Greater, LEqual, GEqual, Equal, NotEqual, Always)] _ZTest ("ZTest", Int) = 0
		[KeywordEnum(Off, On)] _ZWrite ("ZWrite", Int) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue"="Transparent" "RenderQueue"="Overlay+400" "PreviewType"="Plane"
		}

		Cull Off
		LOD 200
		Lighting Off

		Blend Off
		ZWrite On
		ZTest LEqual
		
		Pass
		{
		
			CGPROGRAM

			#include "UnityCG.cginc"
			#pragma multi_compile_fog

			#pragma vertex vert
			#pragma fragment frag
		
			sampler2D _MainTex;
			float4 _MainTex_ST;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}


			float3 Hue(float H)
			{
				float R = abs(H * 6 - 3) - 1;
				float G = 2 - abs(H * 6 - 2);
				float B = 2 - abs(H * 6 - 4);
				return saturate(float3(R,G,B));
			}
		
			float3 HSVtoRGB(in float3 HSV)
			{
				return float3(((Hue(HSV.x) - 1) * HSV.y + 1) * HSV.z);
			}
		
			float3 RGBtoHSV(float3 RGB)
			{
				float3 HSV = 0;
				float M = min(RGB.r, min(RGB.g, RGB.b));
				HSV.z = max(RGB.r, max(RGB.g, RGB.b));
				float C = HSV.z - M;
				if (C != 0)
				{
					HSV.y = C / HSV.z;
					float3 D = (((HSV.z - RGB) / 6) + (C / 2)) / C;
					if (RGB.r == HSV.z)
						HSV.x = D.b - D.g;
					else if (RGB.g == HSV.z)
						HSV.x = (1.0/3.0) + D.r - D.b;
					else if (RGB.b == HSV.z)
						HSV.x = (2.0/3.0) + D.g - D.r;
					if ( HSV.x < 0.0 ) { HSV.x += 1.0; }
					if ( HSV.x > 1.0 ) { HSV.x -= 1.0; }
				}
				return HSV;
			}
		
			int iterations;
			float formuparam;
		
			int volsteps;
			int startstep;
			float stepsize;
		
			float4 position2D;
		
			float zoom;
			float tile;
			float speed;
		
			float brightness;
			float darkmatter;
			float distfading;
			float saturation;
		
			float _Color;
			float _HueShift;
			float _Saturation;
			float _Lightness;
		
			float _AspectRatio;
		
			float2 rotate(float2 p, float a)
			{
				return float2(p.x * cos(a) - p.y * sin(a), p.x * sin(a) + p.y * cos(a));
			}

			fixed4 frag (v2f i) : SV_Target
			{
				// Get coords and direction
			
				float2 uv = i.uv;
				uv = (uv - 0.5) * 2.0;
				uv.x *= _AspectRatio;
			
				uv = rotate(uv, position2D.z);
			
				float3 dir = float3(uv.x * zoom, uv.y * zoom, 1.0);
				float time = _Time.y * speed + 0.25;
			
				float3 from = float3(1.0, 0.5, 0.5);
				from += float3(time * 2.0, time, -2.0);
			
				from += float3(position2D.x * position2D.w, position2D.y * position2D.w, 0.0);
						
				// Volumetric rendering
				float s = 0.1, fade = 1.0;
			
				float3 v = float3(0.0, 0.0, 0.0);
				float3 v2 = float3(0.0, 0.0, 0.0);
			
				for (int i = 0; i < startstep; i++)
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
					if (i > 6) 
					{
						fade *= 1.0 - dm; // Dark matter, don't render near
					}
					// v2 += float3(dm, dm * 0.5, 0.0);
					v2 += fade;
					v2 += float3(s, s * s, s * s * s * s) * a * brightness * fade; // Coloring based on distance
					fade *= distfading; // Distance fading
					s += stepsize;
				}
			
				for (int r = min(startstep, volsteps); r < volsteps; r++)
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
			
				float3 temp = v * 0.01;
			
				temp = RGBtoHSV(temp);
			
				float3 tempCol = RGBtoHSV(v * 0.01);
				tempCol.r = frac(tempCol.r + _HueShift);
				tempCol.g *= _Saturation;
				tempCol.b *= _Lightness;
			
				tempCol = HSVtoRGB(tempCol);
			
				float3 luma = float3(0.299, 0.587, 0.114);	
				float3 d = lerp(dot(luma, tempCol), tempCol, _Color);
			
				float4 col = float4(d.x, d.y, d.z, 1.0);
				UNITY_APPLY_FOG(i.fogCoord, col);
			
				return col;

				//return float4(1.0, 1.0, 1.0, 1.0);
			}

			ENDCG
		}
	} 
	FallBack "Diffuse"
	CustomEditor "CustomStarInspector"
}
