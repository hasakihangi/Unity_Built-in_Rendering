#if !defined(A_TOOLS_INCLUDE)
#define A_TOOLS_INCLUDE
#endif

float4 OutputTestColor(float parameter)
{
    return float4(parameter, parameter, parameter, 1);
}

float4 OutputTestColor(float3 color)
{
    return float4(color, 1);
}

float4 OutputTestColor(float4 outputColor)
{
    return outputColor;
}

void Unity_Remap_float4(float4 In, float2 InMinMax, float2 OutMinMax, out float4 Out)
{
    Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Remap(float3 In, float2 InMinMax, float2 OutMinMax, out float3 Out)
{
    Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Remap(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
{
    Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Remap(float In, float2 InMinMax, float2 OutMinMax, out float Out)
{
    Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}