sampler2D screen : register(s0);

float2 size = float2(0, 0);


float distsqrd(float2 a, float2 b)
{
	float d = a.x - b.x;
	float e = a.y - b.y;
	
	return (d * d + e * e);
}

float4 normal(float4 VertexColor : COLOR, float2 inCoord : TEXCOORD0) : COLOR0
{
	return tex2D(screen, inCoord);
}


technique main
{
    pass normal
    {
		PixelShader = compile ps_2_0 normal();
    }
}
