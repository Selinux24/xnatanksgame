//------- Constants --------
float4x4 xView;
float4x4 xProjection;
float4x4 xWorld;

bool xEnableLighting;
float3 xLightDirection;
float xAmbient;

bool xShowDetail;
float xDetailRepeat;

bool xUseLOD;
float xBlendDistance;
float xBlendWidth;

//------- Texture Samplers --------

Texture xTexture1;
sampler Texture1Sampler = sampler_state 
{
	texture = <xTexture1>;
    AddressU = Wrap;
    AddressV = Wrap;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
};

Texture xDetailTexture1;
sampler DetailTexture1Sampler = sampler_state 
{
	texture = <xDetailTexture1>;
    AddressU = Wrap;
    AddressV = Wrap;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
};

Texture xTexture2;
sampler Texture2Sampler = sampler_state 
{
	texture = <xTexture2>;
    AddressU = Wrap;
    AddressV = Wrap;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
};

Texture xDetailTexture2;
sampler DetailTexture2Sampler = sampler_state 
{
	texture = <xDetailTexture2>;
    AddressU = Wrap;
    AddressV = Wrap;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
};

Texture xTexture3;
sampler Texture3Sampler = sampler_state 
{
	texture = <xTexture3>;
    AddressU = Wrap;
    AddressV = Wrap;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
};

Texture xDetailTexture3;
sampler DetailTexture3Sampler = sampler_state 
{
	texture = <xDetailTexture3>;
    AddressU = Wrap;
    AddressV = Wrap;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
};

Texture xTexture4;
sampler Texture4Sampler = sampler_state 
{
	texture = <xTexture4>;
    AddressU = Wrap;
    AddressV = Wrap;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
};

Texture xDetailTexture4;
sampler DetailTexture4Sampler = sampler_state 
{
	texture = <xDetailTexture4>;
    AddressU = Wrap;
    AddressV = Wrap;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
};

//------- Technique: MultiTextured --------
 
struct MultiTexVertexToPixel
{
    float4 Position         : POSITION;    
    float4 Color            : COLOR0;
    float3 Normal           : TEXCOORD0;
    float4 TextureCoords    : TEXCOORD1;
    float4 LightDirection   : TEXCOORD2;
    float4 TextureWeights   : TEXCOORD3;
    float Depth             : TEXCOORD4;
};
 
struct MultiTexPixelToFrame
{
    float4 Color : COLOR0;
};
 
float3 LuminanceConv = { 0.2125f, 0.7154f, 0.0721f };

MultiTexVertexToPixel MultiTexturedVS( float4 inPos : POSITION, float3 inNormal: NORMAL, float4 inTexCoords: TEXCOORD0, float4 inTexWeights: TEXCOORD1)
{
	MultiTexVertexToPixel Output = (MultiTexVertexToPixel)0;

    float4x4 viewProj = mul (xView, xProjection);
    float4x4 worldViewProj = mul (xWorld, viewProj);

    Output.Position = mul(inPos, worldViewProj);
    Output.Normal = normalize(mul(inNormal, xWorld));
    Output.TextureCoords = inTexCoords;
    Output.TextureWeights = inTexWeights;
    Output.LightDirection.xyz = -xLightDirection;
    Output.LightDirection.w = 1;
    Output.Depth = Output.Position.z;
     
    return Output;
 }
 
MultiTexPixelToFrame MultiTexturedPS(MultiTexVertexToPixel PSIn)
{
    MultiTexPixelToFrame Output = (MultiTexPixelToFrame)0;

    float2 textureCoords = PSIn.TextureCoords;

    float4 color = tex2D(Texture1Sampler, textureCoords) * PSIn.TextureWeights.w;
    color += tex2D(Texture2Sampler, textureCoords) * PSIn.TextureWeights.z;
    color += tex2D(Texture3Sampler, textureCoords) * PSIn.TextureWeights.y;
    color += tex2D(Texture4Sampler, textureCoords) * PSIn.TextureWeights.x;

	if(xUseLOD)
	{
        float2 nearTextureCoords = PSIn.TextureCoords * 2;
        
        float4 nearColor = tex2D(Texture1Sampler, nearTextureCoords) * PSIn.TextureWeights.w;
        nearColor += tex2D(Texture2Sampler, nearTextureCoords) * PSIn.TextureWeights.z;
        nearColor += tex2D(Texture3Sampler, nearTextureCoords) * PSIn.TextureWeights.y;
        nearColor += tex2D(Texture4Sampler, nearTextureCoords) * PSIn.TextureWeights.x;

		if(xShowDetail)
		{
			//Detalle en blanco y negro con la repetición
			float4 detailColor = tex2D(DetailTexture1Sampler, textureCoords * xDetailRepeat) * PSIn.TextureWeights.w;
			detailColor += tex2D(DetailTexture2Sampler, textureCoords * xDetailRepeat) * PSIn.TextureWeights.z;
			detailColor += tex2D(DetailTexture3Sampler, textureCoords * xDetailRepeat) * PSIn.TextureWeights.y;
			detailColor += tex2D(DetailTexture4Sampler, textureCoords * xDetailRepeat) * PSIn.TextureWeights.x;
			
			detailColor = dot((float3)detailColor, LuminanceConv);			
			nearColor.rgb *= detailColor * 2;
		}

	    float blendFactor = clamp((PSIn.Depth - xBlendDistance) / xBlendWidth, 0, 1);
    	Output.Color = color * blendFactor + nearColor * (1 - blendFactor);
	}
	else
	{
		if(xShowDetail)
		{
			//Detalle en blanco y negro con la repetición
			float4 detailColor = tex2D(DetailTexture1Sampler, textureCoords * xDetailRepeat) * PSIn.TextureWeights.w;
			detailColor += tex2D(DetailTexture2Sampler, textureCoords * xDetailRepeat) * PSIn.TextureWeights.z;
			detailColor += tex2D(DetailTexture3Sampler, textureCoords * xDetailRepeat) * PSIn.TextureWeights.y;
			detailColor += tex2D(DetailTexture4Sampler, textureCoords * xDetailRepeat) * PSIn.TextureWeights.x;
			
			detailColor = dot((float3)detailColor, LuminanceConv);			
			color.rgb *= detailColor * 2;
		}

		Output.Color = color;
	}

    if (xEnableLighting)
    {
        Output.Color *= saturate(dot(PSIn.Normal, PSIn.LightDirection)) * xAmbient;
    }

    return Output;
}
 
technique MultiTextured
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 MultiTexturedVS();
        PixelShader = compile ps_2_0 MultiTexturedPS();
    }
}
