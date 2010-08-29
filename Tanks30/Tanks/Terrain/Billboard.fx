//-----------------------------------------------------------------------------
// Billboard.fx
//
// Microsoft Game Technology Group
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------


float4x4 World;
float4x4 View;
float4x4 Projection;

bool EnableLighting;
float3 LightDirection;
float Ambient;

float3 WindDirection = float3(1, 0, 0);
float WindWaveSize = 0.1;
float WindRandomness = 1;
float WindSpeed = 4;
float WindAmount;
float WindTime;

float BillboardWidth;
float BillboardHeight;

texture Texture;

sampler TextureSampler = sampler_state
{
    Texture = (Texture);

    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VS_INPUT
{
    float3 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TexCoord : TEXCOORD0;
    float Random : TEXCOORD1;
};

struct VS_OUTPUT
{
	float4 Position         : POSITION;    
    float4 Color            : COLOR0;
    float3 Normal           : TEXCOORD0;
    float2 TextureCoords    : TEXCOORD1;
    float4 LightDirection   : TEXCOORD2;
};


VS_OUTPUT VertexShader(VS_INPUT input)
{
    VS_OUTPUT output = (VS_OUTPUT)0;

	//Factor para hacer los billboards de diferentes tamaños
    float squishFactor = 0.75 + abs(input.Random) / 2;

	//Aplicar factor
    float width = BillboardWidth * squishFactor;
    float height = BillboardHeight / squishFactor;

	//Darle la vuelta a la anchura para que aparezca la textura de vez en cuando al revés
    if (input.Random < 0)
    {
        width = -width;
    }

    //Obtener la dirección de la vista
    float3 viewDirection = View._m02_m12_m22;

	//Obtener el vector derecha
    float3 rightVector = normalize(cross(viewDirection, input.Normal));

    //Se calcula la posición usando las coordenadas de textura
    float3 position = mul(input.Position, World);
    position += rightVector * (input.TexCoord.x - 0.5) * width;
    position += input.Normal * (1 - input.TexCoord.y) * height;

    //Calcular cuánto debe afectar al vértice la acción del viento
    float waveOffset = (dot(position, WindDirection) * WindWaveSize) + (input.Random * WindRandomness);
    
    //El viento hace mover los vértices de adelante a atrás, pero sólo a los vértices superiores
    float wind = sin(WindTime * WindSpeed + waveOffset) * WindAmount;
    wind *= (1 - input.TexCoord.y);
    position += WindDirection * wind;

    //Aplicar la transformación de la cámara
    float4 viewPosition = mul(float4(position, 1), View);

    output.Position = mul(viewPosition, Projection);
    output.Normal = input.Normal;
    output.TextureCoords = input.TexCoord;
    output.LightDirection.xyz = -LightDirection;
    output.LightDirection.w = 1;
    
    return output;
}


float4 PixelShader(VS_OUTPUT input) : COLOR0
{
    float4 outputColor = tex2D(TextureSampler, input.TextureCoords);
    
    if (EnableLighting)
    {
        outputColor.rgb *= saturate(dot(input.Normal, input.LightDirection)) * Ambient;
    }
    
    return outputColor;
}

technique Billboards
{
    pass RenderOpaquePixels
    {
        VertexShader = compile vs_1_1 VertexShader();
        PixelShader = compile ps_1_1 PixelShader();

        AlphaBlendEnable = false;
        
        AlphaTestEnable = true;
        AlphaFunc = Equal;
        AlphaRef = 255;
        
        ZEnable = true;
        ZWriteEnable = true;

        CullMode = None;
    }

    pass RenderAlphaBlendedFringes
    {
        VertexShader = compile vs_1_1 VertexShader();
        PixelShader = compile ps_1_1 PixelShader();
        
        AlphaBlendEnable = true;
        SrcBlend = SrcAlpha;
        DestBlend = InvSrcAlpha;
        
        AlphaTestEnable = true;
        AlphaFunc = NotEqual;
        AlphaRef = 255;

        ZEnable = true;
        ZWriteEnable = false;

        CullMode = None;
    }
    
    pass Reset
    {
        AlphaBlendEnable = false;
        AlphaTestEnable = false;
        ZEnable = true;
        ZWriteEnable = true;

        CullMode = CCW;
    }
}
