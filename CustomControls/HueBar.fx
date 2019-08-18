// Compile with: "C:\Program Files (x86)\Windows Kits\...\bin\...\x86\fxc.exe" /T ps_3_0 /E main /Fo HueBar.ps HueBar.fx

#define MAGNIFIER 1

static const float PI = 3.14159265f;

sampler2D Input : register(s0);
float CenterSize : register(c0);
float CursorPosition : register(c1);
float CursorSize : register(c2);
float4 CenterDotColor : register(c3);
float CenterDotSize : register(c4);
float2 AA : register(c5); // AntiAliasingParameters

float4 main(float2 uv : TEXCOORD) : COLOR
{
    uv.x = uv.x * 2 - 1;
    uv.y = uv.y * 2 - 1;
    float d2 = uv.x * uv.x + uv.y * uv.y;
    if (d2 > 1) return float4(0, 0, 0, 0);
    float d;
    float2 n;
    float nd;
    if (d2 < CenterDotSize * CenterDotSize)
    {
        d = sqrt(d2);
        n = (uv / d) * AA;
        nd = sqrt(n.x * n.x + n.y * n.y);
        float4 color = CenterDotColor;
        if (d > CenterDotSize - nd) color *= ((CenterDotSize - d) / nd);
        color *= tex2D(Input, uv.xy).a;
        return color;
    }
    if (CursorSize == 0 && d2 < CenterSize * CenterSize) return float4(0, 0, 0, 0);

    d = sqrt(d2);
    float Hue = acos(uv.y / d) * (3 / PI);
    if (uv.x < 0) Hue += 3;
    else Hue = 3 - Hue;

    float s = min(abs(CursorPosition - Hue), abs(CursorPosition - (Hue - 6)));
    s = min(s, abs((CursorPosition - 6) - Hue));
    if (s > 0.5) s = 0;
    else
    {
        s *= 2;
        s = 1 - s * s;
        s = s * s;
    }

    float s_effect = s * CursorSize;
    float center_threshold = CenterSize - s_effect / 4;

    if (d < center_threshold) return float4(0, 0, 0, 0);

#if MAGNIFIER
    if (s > 0)
    {
        float CP = CursorPosition;
        float a = abs(CP - Hue);
        if (abs(CP + 6 - Hue) < a) CP += 6;
        else if (abs(CP - 6 - Hue) < a) CP -= 6;
        float temp = 1 - s_effect;
        temp *= temp;
        temp *= temp;
        temp = 1 - temp;
        Hue = lerp(Hue, CP, temp);
        if (Hue > 6) Hue -= 6;
        else if (Hue < 0) Hue += 6;
    }
#endif

    float4 color;
    if (Hue < 1) // [0, 1)
        color = float4(1, Hue, 0, 1);     // R = 1, G = 0=>1
    else if (Hue < 2) // [1, 2)
        color = float4(2 - Hue, 1, 0, 1); // G = 1, R = 1=>0
    else if (Hue < 3) // [2, 3)
        color = float4(0, 1, Hue - 2, 1); // G = 1, B = 0=>1
    else if (Hue < 4) // [3, 4)
        color = float4(0, 4 - Hue, 1, 1); // B = 1, G = 1=>0
    else if (Hue < 5) // [4, 5)
        color = float4(Hue - 4, 0, 1, 1); // B = 1, R = 0=>1
    else // [5, 6)
        color = float4(1, 0, 6 - Hue, 1); // R = 1, B = 1=>0

    n = (uv / d) * AA;
    nd = sqrt(n.x * n.x + n.y * n.y);

    if (d > 1 - nd) color *= ((1 - d) / nd);
    else if (d < center_threshold + nd + s)
    {
        if (CursorSize != 0 && s != 0)
        {
            if (s > 0.5) s = 1 - s;
            s *= 2;
            s = 1 - s * s;
            s = 1 - s * s;
            nd *= 1 + s_effect * 0.2;
        }
        if (d < center_threshold + nd) color *= ((d - center_threshold) / nd);
    }

    color *= tex2D(Input, uv.xy).a;
    return color;
}
