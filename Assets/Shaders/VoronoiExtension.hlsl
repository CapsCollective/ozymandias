#define VTIME 0.5
#define SMOOTHING 0.171

float2 VoronoiHash(float2 p)
{
	p = float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)));
	return frac(sin(p) * 43758.5453);
}


float Voronoi(float2 v, float time, float smoothness)
{
#if defined(CLOUDS_ENABLED)
	float2 n = floor(v);
	float2 f = frac(v);
	float F1 = 8.0;
	float F2 = 8.0;
	for (int j = -1; j <= 1; j++)
	{
		for (int i = -1; i <= 1; i++)
		{
			float2 g = float2(i, j);
			float2 o = VoronoiHash(n + g);
			o = (sin(time + o * 6.2831) * 0.5 + 0.5); float2 r = f - g - o;
			float d = 0.5 * dot(r, r);
			//		if( d<F1 ) {
			//			F2 = F1;
			float h = smoothstep(0.0, 1.0, 0.5 + 0.5 * (F1 - d) / smoothness); F1 = lerp(F1, d, h) - smoothness * h * (1.0 - h);
			//		} else if( d<F2 ) {
			//			F2 = d;

			//		}
		}
	}
	return F1;
#endif
	return 0;
}

void Voronoii_float(float2 v, float time, float smoothness, inout float Out)
{
	float vor = 0;
	float fade = 0.5;
	float rest = 0;
	
	// Uncomment for more detailed clouds
	//for (int i = 0; i < 2; i++) {
	vor += Voronoi(v, time, smoothness);
	//	rest += fade;
	//	v *= 2;
	//	fade *= 0.5;
	//}
	Out = vor;
}

float3 VoronoiColor_float(inout float3 color, float2 v, float time, float smoothness, float darkness, float smoothing)
{
	float o = 0.0;
	Voronoii_float(v, time * VTIME, smoothness, o);
	o = saturate(smoothstep(SMOOTHING, SMOOTHING + 0.02, o));
	color = lerp(color, float3(0, 0, 0), o * darkness);
	return color;
}

void VoronoiColor_float(in float3 color, float2 v, float time, float smoothness, float darkness, float smoothing, out float3 Out)
{
	float o = 0;
	Voronoii_float(v, time * VTIME, smoothness, o);
	o = saturate(smoothstep(SMOOTHING, SMOOTHING + 0.02, o));
	color = lerp(color, float3(0, 0, 0), o * darkness);
	Out = color;
}

void VoronoiSample_float(in float3 color, float2 v, float time, float smoothness, float darkness, out float3 Out)
{
	float o = 0;
	Voronoii_float(v, time * VTIME, smoothness, o);
	Out = o;
}