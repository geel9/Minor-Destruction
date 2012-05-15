float4 colorTop = float4(0.9, 0.9, 0.9, 1);
float4 colorBottom = float4(0.86, 0.86, 0.86, 1);

bool hovering = false;

float4 normal(float4 VertexColor : COLOR, float2 inCoord : TEXCOORD0) : COLOR0
{
	float which = (inCoord.y / 0.05);
	float4 color;

	int thresh = 10;

	if(which < thresh)
		color = colorTop;
	else
		color = colorBottom;

	which = round(which);

	int divisor = 100;
	if(hovering)
		divisor -= 30;

	which /= divisor;

	color -= which;
	color.a = 1;


	return color;
}

technique main
{
    pass normal
    {
		PixelShader = compile ps_2_0 normal();
    }
}

