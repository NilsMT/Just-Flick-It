Shader"Hidden/LessDistortedVHSEffect" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _VHSTex ("Base (RGB)", 2D) = "white" {}
    }

    SubShader {
        Pass {
ZTest Always
Cull Off
ZWrite Off
            Fog {
Mode off}

            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

uniform sampler2D _MainTex;
uniform sampler2D _VHSTex;

float _yScanline;
float _xScanline;
float rand(float3 co)
{
    return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
}

fixed4 frag(v2f_img i) : COLOR
{
    fixed4 vhs = tex2D(_VHSTex, i.uv);

                // Reducing distortion effect
    float dx = 1 - abs(distance(i.uv.y, _xScanline));
    float dy = 1 - abs(distance(i.uv.y, _yScanline));

    dy = ((int) (dy * 7)) / 7.0;
    i.uv.x += dy * 0.01 + rand(float3(dy, dy, dy)).r / 200;

    float white = (vhs.r + vhs.g + vhs.b) / 3;

    if (dx > 0.99)
        i.uv.y = _xScanline;

    i.uv.x = i.uv.x % 1;
    i.uv.y = i.uv.y % 1;

    fixed4 c = tex2D(_MainTex, i.uv);

    float x = ((int) (i.uv.x * 320)) / 320.0;
    float y = ((int) (i.uv.y * 240)) / 240.0;

    c -= rand(float3(x, y, _xScanline)) * _xScanline / 5;
    return c + vhs;
}
            ENDCG
        }
    }
Fallback off
}
