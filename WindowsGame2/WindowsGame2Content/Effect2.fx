sampler2D screen : register(s0);

int time = 0;
bool submerged = false;

float4 normal(float4 VertexColor : COLOR, float2 inCoord : TEXCOORD0) : COLOR0
{
	return float4(0, 0, 0.1, 0) + VertexColor;
}


technique main
{
    pass normal
    {
		PixelShader = compile ps_2_0 normal();
    }
}
