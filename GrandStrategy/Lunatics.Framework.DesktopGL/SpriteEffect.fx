#define TECHNIQUE(name, vsname, psname ) \
	technique name { pass { VertexShader = compile vs_2_0 vsname (); PixelShader = compile ps_2_0 psname(); } }

#define BEGIN_CONSTANTS
#define MATRIX_CONSTANTS
#define END_CONSTANTS

#define _vs(r)  : register(vs, r)
#define _ps(r)  : register(ps, r)
#define _cb(r)

#define DECLARE_TEXTURE(Name, index) \
    sampler2D Name : register(s##index);

#define DECLARE_CUBEMAP(Name, index) \
    samplerCUBE Name : register(s##index);

#define SAMPLE_TEXTURE(Name, texCoord)  tex2D(Name, texCoord)
#define SAMPLE_CUBEMAP(Name, texCoord)  texCUBE(Name, texCoord)



DECLARE_TEXTURE(Texture, 0);
sampler2D Texture : register(s0);

float4x4 MatrixTransform    register(vs, c0);


struct VSOutput
{
	float4 position		: POSITION;
	float4 color		: COLOR0;
	float2 texCoord		: TEXCOORD0;
};

VSOutput SpriteVertexShader(
	float4 position	: POSITION0,
	float4 color : COLOR0,
	float2 texCoord : TEXCOORD0)
{
	VSOutput output;
	output.position = mul(position, MatrixTransform);
	output.color = color;
	output.texCoord = texCoord;
	return output;
}

float4 SpritePixelShader(VSOutput input) : SV_Target0
{
	return tex2D(Texture, input.texCoord) * input.color;
}


technique SpriteBatch {
	pass { 
		VertexShader = compile vs_2_0 SpriteVertexShader();
		PixelShader = compile ps_2_0 SpritePixelShader();
	} 
}
