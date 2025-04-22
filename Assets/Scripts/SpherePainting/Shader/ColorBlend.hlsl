//reference from https://www.w3.org/TR/compositing-1/#valdef-blend-mode-screen

// composite
float4 SourceOver(float4 backdrop, float4 source)
{
    float3 co = source.w * source.rgb + backdrop.w * backdrop.rgb * (1.0 - source.w);
    float ao = source.w + backdrop.w * (1.0 - source.w);
    return float4(co / ao, ao);
}

// blend
inline float Multiply(float backdrop, float source)
{
    return backdrop * source;
}

inline float3 Multiply(float3 backdrop, float3 source)
{
    return backdrop * source;
}

inline float Screen(float backdrop, float source)
{
    return backdrop + source - (backdrop * source);
}

inline float Hardlight(float backdrop, float source)
{
    return source <= 0.5 ? Multiply(backdrop, 2.0 * source)
                         : Screen(backdrop, 2.0 * source - 1.0);
}

float3 Hardlight(float3 backdrop, float3 source)
{
    return float3(Hardlight(backdrop.r, source.r),
                  Hardlight(backdrop.g, source.g),
                  Hardlight(backdrop.b, source.b));
}

float3 Overlay(float3 backdrop, float3 source)
{
    return Hardlight(source, backdrop);
}

inline float3 Add(float3 backdrop, float3 source)
{
    return min(1.0, backdrop.rgb + source.rgb);
}

inline float3 Difference(float3 backdrop, float3 source)
{
    return abs(backdrop.rgb - source.rgb);
}

inline float3 Exclusion(float3 backdrop, float3 source)
{
    return backdrop + source - 2.0 * backdrop * source;
}

inline float Lum(float3 color)
{
    return 0.3 * color.r + 0.59 * color.g + 0.11 * color.b;
}

float3 ClipColor(float3 color)
{
    float lum = Lum(color);
    float n = min(color.r, min(color.g, color.b));
    float x = max(color.r, max(color.g, color.b));
    if(n < 0.0)
    {
        color = lum + (((color - lum) * lum) / (lum - n));
    }

    if(x > 1.0)
    {
        color = lum + (((color - lum) * (1.0 - lum)) / (x - lum));
    }

    return color;
}

inline float3 SetLum(float3 color, float lum)
{
    float d = lum - Lum(color);
    color.r += d;
    color.g += d;
    color.b += d;
    return ClipColor(color);
}

float Sat(float3 color)
{
    return max(color.r, max(color.g, color.b)) - min(color.r, min(color.g, color.b));
}

float3 SetSat(float3 color, float sat)
{
    float cmax = max(color.r, max(color.g, color.b));
    float cmin = min(color.r, min(color.g, color.b));
    float cmid = color.r + color.g + color.b - cmax - cmin;

    if (cmax > cmin)
    {
        cmid = ((cmid - cmin) * sat) / (cmax - cmin);
        cmax = sat;
    }
    else
    {
        cmid = 0.0;
        cmax = 0.0;
    }
    cmin = 0.0;


    // 結果の色を決定（等号を使わずに比較する）
    float3 result;
    if (color.r >= color.g && color.r >= color.b)
        result.r = cmax;
    else if (color.r <= color.g && color.r <= color.b)
        result.r = cmin;
    else
        result.r = cmid;

    if (color.g >= color.r && color.g >= color.b)
        result.g = cmax;
    else if (color.g <= color.r && color.g <= color.b)
        result.g = cmin;
    else
        result.g = cmid;

    if (color.b >= color.r && color.b >= color.g)
        result.b = cmax;
    else if (color.b <= color.r && color.b <= color.g)
        result.b = cmin;
    else
        result.b = cmid;
        
    return result;
}

float3 Hue(float3 backdrop, float3 source)
{
    return SetLum(SetSat(source, Sat(backdrop)), Lum(backdrop));
}

float3 Saturation(float3 backdrop, float3 source)
{
    return SetLum(SetSat(backdrop, Sat(source)), Lum(backdrop));
}

float3 Color(float3 backdrop, float3 source)
{
    return SetLum(source, Lum(backdrop));
}

float3 Luminosity(float3 backdrop, float3 source)
{
    return SetLum(backdrop, Lum(source));
}

float4 CompositeColor(float4 backdrop, float4 source, int blendMode)
{
    float3 blendColor = source.rgb;
    float Fb = 1.0 - source.w;
    float Ff = 1.0;

    switch(blendMode)
    {
        case 0: // NORMAL
            blendColor = source.rgb;
        break;
        case 1: // MULTIPLY
            blendColor = Multiply(backdrop.rgb, source.rgb);
        break;
        case 2: // OVERLAY
            blendColor = Overlay(backdrop.rgb, source.rgb);
        break;
        case 3: // ADD
            blendColor = Add(backdrop.rgb, source.rgb);
        break;
        case 4: // ADD_GLOW
            Fb = 1.0;
            Ff = 1.0;
            blendColor = source.rgb;
        break;
        case 5: // DIFFERENCE
            blendColor = Difference(backdrop.rgb, source.rgb);
        break;
        case 6: // EXCLUSION
            blendColor = Exclusion(backdrop.rgb, source.rgb);
        break;
        case 7: // HUE
            blendColor = Hue(backdrop.rgb, source.rgb);
        break;
        case 8: // SATURATION
            blendColor = Saturation(backdrop.rgb, source.rgb);
        break;
        case 9: // COLOR
            blendColor = Color(backdrop.rgb, source.rgb);
        break;
        case 10: // LUMINOSITY
            blendColor = Luminosity(backdrop.rgb, source.rgb);
        break;
    }

    float alpha = backdrop.w * Fb + source.w * Ff;
    float3 CfPrime = backdrop.w * blendColor + (1.0 - backdrop.w) * source.rgb;
    float3 color = backdrop.w * Fb * backdrop.rgb + source.w * Ff * CfPrime;
    color /= alpha;
    return float4(color, alpha);
}

float4 InverseNormalBlend(float4 result, float4 backdrop, float sourceOpacity)
{
    float4 source;
    source.w = sourceOpacity;
    source.rgb = (result.w * result.rgb - backdrop.w * (1 - source.w) * backdrop.rgb) / source.w;
    return source;
}