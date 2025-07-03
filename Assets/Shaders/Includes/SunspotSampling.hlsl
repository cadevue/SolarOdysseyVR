struct Sunspot
{
    float2 uv;
    float rotation;
    float scale;
    int layer;
};

StructuredBuffer<Sunspot> _SunspotBuffer;

float2 RotateUV(float2 uv, float2 center, float angle)
{
    float s = sin(angle);
    float c = cos(angle);
    uv -= center;
    uv = float2(uv.x * c - uv.y * s, uv.x * s + uv.y * c);
    return uv + center;
}

void SampleSunspots_float(float2 uv, out float mask)
{
    mask = 0;

    [unroll(20)]
    for (int i = 0; i < 20; i++)
    {
        Sunspot s = _SunspotBuffer[i];
        if (s.scale <= 0) continue;

        float2 tuv = (uv - s.uv) / s.scale + 0.5;
        tuv = RotateUV(tuv, 0.5, s.rotation);

        float4 tex = SAMPLE_TEXTURE2D_ARRAY(_SunspotTexArray, sampler_SunspotTexArray, tuv, s.layer);

        mask = max(mask, tex.r); // Use max, or add for accumulation
    }
}
