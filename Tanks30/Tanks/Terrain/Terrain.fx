//------- Constants --------
float4x4 xView;
float4x4 xProjection;
float4x4 xWorld;
bool xEnableLighting;
float xAmbient;
float3 xLightDirection;

//------- Texture Samplers --------

Texture xTexture;
sampler TextureSampler = sampler_state 
{ 
	texture = <xTexture>;
    AddressU = Wrap;
    AddressV = Wrap;
    MinFilter = Point;
    MagFilter = Linear;
    MipFilter = Linear;
};

Texture xTexture1;
sampler Texture1Sampler = sampler_state 
{
	texture = <xTexture1>;
    AddressU = Wrap;
    AddressV = Wrap;
    MinFilter = Point;
    MagFilter = Linear;
    MipFilter = Linear;
};

Texture xDetailTexture1;
sampler DetailTexture1Sampler = sampler_state 
{
	texture = <xDetailTexture1>;
    AddressU = Wrap;
    AddressV = Wrap;
    MinFilter = Point;
    MagFilter = Linear;
    MipFilter = Linear;
};

Texture xTexture2;
sampler Texture2Sampler = sampler_state 
{
	texture = <xTexture2>;
    AddressU = Wrap;
    AddressV = Wrap;
    MinFilter = Point;
    MagFilter = Linear;
    MipFilter = Linear;
};

Texture xDetailTexture2;
sampler DetailTexture2Sampler = sampler_state
{
	texture = <xDetailTexture2>;
    AddressU = Wrap;
    AddressV = Wrap;
    MinFilter = Point;
    MagFilter = Linear;
    MipFilter = Linear;
};

Texture xTexture3;
sampler Texture3Sampler = sampler_state 
{
	texture = <xTexture3>;
    AddressU = Wrap;
    AddressV = Wrap;
    MinFilter = Point;
    MagFilter = Linear;
    MipFilter = Linear;
};

Texture xDetailTexture3;
sampler DetailTexture3Sampler = sampler_state 
{
	texture = <xDetailTexture3>;
    AddressU = Wrap;
    AddressV = Wrap;
    MinFilter = Point;
    MagFilter = Linear;
    MipFilter = Linear;
};

Texture xTexture4;
sampler Texture4Sampler = sampler_state 
{
	texture = <xTexture4>;
    AddressU = Wrap;
    AddressV = Wrap;
    MinFilter = Point;
    MagFilter = Linear;
    MipFilter = Linear;
};

Texture xDetailTexture4;
sampler DetailTexture4Sampler = sampler_state
{
	texture = <xDetailTexture4>;
    AddressU = Wrap;
    AddressV = Wrap;
    MinFilter = Point;
    MagFilter = Linear;
    MipFilter = Linear;
};

float3 LuminanceConv = { 0.2125f, 0.7154f, 0.0721f };

//------- Technique: Textured --------

struct TexVertexToPixel
{
    float4 Position         : POSITION;    
    float4 Color            : COLOR0;
    float3 Normal           : TEXCOORD0;
    float2 TextureCoords    : TEXCOORD1;
    float4 LightDirection   : TEXCOORD2;

};

struct TexPixelToFrame
{
    float4 Color : COLOR0;
};

TexVertexToPixel TexturedVS( float4 inPos : POSITION, float3 inNormal: NORMAL, float2 inTexCoords: TEXCOORD0)
{
    TexVertexToPixel Output = (TexVertexToPixel)0;
    float4x4 preViewProjection = mul (xView, xProjection);
    float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
    
    Output.Position = mul(inPos, preWorldViewProjection);
    Output.Normal = mul(normalize(inNormal), xWorld);
    Output.TextureCoords = inTexCoords;
    Output.LightDirection.xyz = -xLightDirection;
    Output.LightDirection.w = 1;
    
    return Output;    
}

TexPixelToFrame TexturedPS(TexVertexToPixel PSIn)
{
    TexPixelToFrame Output = (TexPixelToFrame)0;
    
    float lightingFactor = 1;
    if (xEnableLighting)
        lightingFactor = saturate(saturate(dot(PSIn.Normal, PSIn.LightDirection)) + xAmbient);

    Output.Color = tex2D(TextureSampler, PSIn.TextureCoords)*lightingFactor;

    return Output;
}

technique Textured
{
    pass Pass0
    {
        VertexShader = compile vs_1_1 TexturedVS();
        PixelShader = compile ps_1_1 TexturedPS();
    }
}


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
 
 MultiTexVertexToPixel MultiTexturedVS( float4 inPos : POSITION, float3 inNormal: NORMAL, float4 inTexCoords: TEXCOORD0, float4 inTexWeights: TEXCOORD1)
 {
     MultiTexVertexToPixel Output = (MultiTexVertexToPixel)0;
     float4x4 preViewProjection = mul (xView, xProjection);
     float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
     
     Output.Position = mul(inPos, preWorldViewProjection);
     Output.Normal = normalize(mul(inNormal, xWorld));
     Output.TextureCoords = inTexCoords;
     Output.LightDirection.xyz = -xLightDirection;
     Output.LightDirection.w = 1;
     Output.TextureWeights = inTexWeights;
     Output.Depth = Output.Position.z;
     
     return Output;    
 }
 
 MultiTexPixelToFrame MultiTexturedPS(MultiTexVertexToPixel PSIn)
 {
     MultiTexPixelToFrame Output = (MultiTexPixelToFrame)0;

     float lightingFactor = 1;
     if (xEnableLighting)
     {
         lightingFactor = saturate(saturate(dot(PSIn.Normal, PSIn.LightDirection)) + xAmbient);
     }
     else
     {
         lightingFactor = saturate(1 + xAmbient);
     }

     const float detailRepeat = 7.5;

     float2 textureCoords = PSIn.TextureCoords;

     float4 color1 = tex2D(Texture1Sampler, textureCoords);
     float4 color2 = tex2D(Texture2Sampler, textureCoords);
     float4 color3 = tex2D(Texture3Sampler, textureCoords);
     float4 color4 = tex2D(Texture4Sampler, textureCoords);

     float4 detailColor1 = tex2D(DetailTexture1Sampler, textureCoords * detailRepeat);
     float4 detailColor2 = tex2D(DetailTexture2Sampler, textureCoords * detailRepeat);
     float4 detailColor3 = tex2D(DetailTexture3Sampler, textureCoords * detailRepeat);
     float4 detailColor4 = tex2D(DetailTexture4Sampler, textureCoords * detailRepeat);

     color1.rgb *= detailColor1 * 2;
     color2.rgb *= detailColor2 * 2;
     color3.rgb *= detailColor3 * 2;
     color4.rgb *= detailColor4 * 2;

     float4 color;
     color = color1 * PSIn.TextureWeights.w;
     color += color2 * PSIn.TextureWeights.z;
     color += color3 * PSIn.TextureWeights.y;
     color += color4 * PSIn.TextureWeights.x;
 
     Output.Color = saturate(color * lightingFactor);

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

