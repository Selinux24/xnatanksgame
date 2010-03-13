//------- Constants --------
float4x4 xView;
float4x4 xProjection;
float4x4 xWorld;
float3 xLightDirection;
float xAmbient;
bool xEnableLighting;

//------- Texture Samplers --------

Texture xTexture;
sampler TextureSampler = sampler_state { texture = <xTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture xGrassTexture;
sampler GrassTextureSampler = sampler_state { texture = <xGrassTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture xSandTexture;
sampler SandTextureSampler = sampler_state { texture = <xSandTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture xRockTexture;
sampler RockTextureSampler = sampler_state { texture = <xRockTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture xSnowTexture;
sampler SnowTextureSampler = sampler_state { texture = <xSnowTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};


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
     Output.Normal = mul(normalize(inNormal), xWorld);
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

     float blendDistance = 250;
     float blendWidth = 10;
     float blendFactor = clamp((PSIn.Depth-blendDistance)/blendWidth, 0, 1);
     
     float lightingFactor = 1;
     if (xEnableLighting)
         lightingFactor = saturate(saturate(dot(PSIn.Normal, PSIn.LightDirection)) + xAmbient);
 
     float4 farColor;
     farColor = tex2D(SandTextureSampler, PSIn.TextureCoords)*PSIn.TextureWeights.x;
     farColor += tex2D(GrassTextureSampler, PSIn.TextureCoords)*PSIn.TextureWeights.y;
     farColor += tex2D(RockTextureSampler, PSIn.TextureCoords)*PSIn.TextureWeights.z;
     farColor += tex2D(SnowTextureSampler, PSIn.TextureCoords)*PSIn.TextureWeights.w;
 
     float4 nearColor;
     float2 nearTextureCoords = PSIn.TextureCoords*3;
     nearColor = tex2D(SandTextureSampler, nearTextureCoords)*PSIn.TextureWeights.x;
     nearColor += tex2D(GrassTextureSampler, nearTextureCoords)*PSIn.TextureWeights.y;
     nearColor += tex2D(RockTextureSampler, nearTextureCoords)*PSIn.TextureWeights.z;
     nearColor += tex2D(SnowTextureSampler, nearTextureCoords)*PSIn.TextureWeights.w;
 
     Output.Color = farColor*blendFactor + nearColor*(1-blendFactor);
     Output.Color *= saturate(lightingFactor + xAmbient);

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

