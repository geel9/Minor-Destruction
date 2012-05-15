sampler2D screen : register(s0);
float aspect = 129 / 40;
float2 middle = float2(0.5, 0.5);
float4 outlineColor = float4(1,1,1,1);

float cutoff = 0;

float distsqrd(float2 a, float2 b)
{
	float d = a.x - b.x;
	float e = a.y - b.y;
	d *= aspect;
	
	return (d * d + e * e);
}


float4 normal(float4 VertexColor : COLOR, float2 inCoord : TEXCOORD0) : COLOR0
{
	float d = distsqrd(inCoord, middle);
	if(d <= 0.24)
		return VertexColor - (d * cutoff);

	else if(d > 0.24 && d <= 0.25)
		//return outlineColor;
		return float4(0,0,0,0);

	else
		return float4(0,0,0,0);
}

technique main
{
    pass normal
    {
		PixelShader = compile ps_2_0 normal();
    }
}
