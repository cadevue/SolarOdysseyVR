// 3D Simplex Noise - Unity HLSL version
// Original: https://www.shadertoy.com/view/XsX3zB

float3 random3(float3 c) {
    float j = 4096.0 * sin(dot(c, float3(17.0, 59.4, 15.0)));
    float3 r;
    r.z = frac(512.0 * j);
    j *= 0.125;
    r.x = frac(512.0 * j);
    j *= 0.125;
    r.y = frac(512.0 * j);
    return r - 0.5;
}

#define F3 0.3333333
#define G3 0.1666667

float simplex3d(float3 p) {
    float3 s = floor(p + dot(p, float3(F3, F3, F3)));
    float3 x = p - s + dot(s, float3(G3, G3, G3));

    float3 e = step(float3(0.0, 0.0, 0.0), x - x.yzx);
    float3 i1 = e * (1.0 - e.zxy);
    float3 i2 = 1.0 - e.zxy * (1.0 - e);

    float3 x1 = x - i1 + G3;
    float3 x2 = x - i2 + 2.0 * G3;
    float3 x3 = x - 1.0 + 3.0 * G3;

    float4 w;
    w.x = dot(x, x);
    w.y = dot(x1, x1);
    w.z = dot(x2, x2);
    w.w = dot(x3, x3);

    w = max(0.6 - w, 0.0);

    float4 d;
    d.x = dot(random3(s), x);
    d.y = dot(random3(s + i1), x1);
    d.z = dot(random3(s + i2), x2);
    d.w = dot(random3(s + 1.0), x3);

    w *= w;
    w *= w;
    d *= w;

    return dot(d, float4(52.0, 52.0, 52.0, 52.0));
}

float3x3 rot1 = float3x3(-0.37, 0.36, 0.85,
                         -0.14, -0.93, 0.34,
                          0.92, 0.01, 0.4);
float3x3 rot2 = float3x3(-0.55, -0.39, 0.74,
                          0.33, -0.91, -0.24,
                          0.77, 0.12, 0.63);
float3x3 rot3 = float3x3(-0.71, 0.52, -0.47,
                         -0.08, -0.72, -0.68,
                         -0.7, -0.45, 0.56);

float simplex3d_fractal(float3 m) {
    return 0.5333333 * simplex3d(mul(rot1, m))
         + 0.2666667 * simplex3d(2.0 * mul(rot2, m))
         + 0.1333333 * simplex3d(4.0 * mul(rot3, m))
         + 0.0666667 * simplex3d(8.0 * m);
}

void Fractal3DNoise_float(float3 p, float noiseScale, out float value)
{
    float3 scaledP = p;
    value = simplex3d_fractal(scaledP);
    value = 0.5 * value + 0.5; // Normalize to [0, 1]
}
