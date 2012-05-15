sampler2D screen : register(s0);

float height = 30;
bool override = false;

float4 normal(float4 VertexColor : COLOR, float2 inCoord : TEXCOORD0) : COLOR0
{
	if(height == 100 || override){
		return float4(0, 0, 1, 0.8);
	}

	float realHeight = height / 100;
	if(abs(inCoord.y - realHeight) <= 0.05){
		return float4(0, 0, 0, 0.8);
	}
	else if (inCoord.y < realHeight){
		return float4(0, 0, 1, 0.8);
	}
	return float4(1,1,1,0);
}


technique main
{
    pass normal
    {
		PixelShader = compile ps_2_0 normal();
    }
}
