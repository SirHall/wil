#include "LitForwardPassStripped.hlsl"

#ifndef CORAL_COLOR_FRAGMENT_INCLUDED
#define CORAL_COLOR_FRAGMENT_INCLUDED

//--- Mandelbrot Functions Start ---//

half logInterp(half2 pos, float i, float iter)
{
    float modulus = sqrt(pos.x * pos.x + pos.y * pos.y);
    return i + 1 - (log2(log2(modulus))) / 2.0;
}

half3 hsv2rgb(half3 c)
{
    half4 K = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    half3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

half3 to3(half v)
{
    return half3(v, v, v);
}

half3 to3(half x, half y)
{
    return half3(x, y, 0.0);
}

half f_mod(half a, half b)
{
  half c = frac(abs(a/b))*abs(b);
  return (a < 0) ? -c : c;   /* if ( a < 0 ) c = 0-c */
}

//--- Mandelbrot Functions End ---//


half4 LitPassFragment(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

#if defined(_PARALLAXMAP)
#if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
    half3 viewDirTS = input.viewDirTS;
#else
    half3 viewDirTS = GetViewDirectionTangentSpace(input.tangentWS, input.normalWS, input.viewDirWS);
#endif
    ApplyPerPixelDisplacement(viewDirTS, input.uv);
#endif

    SurfaceData surfaceData;
    InitializeStandardLitSurfaceData(input.uv, surfaceData);

    InputData inputData;
    InitializeInputData(input, surfaceData.normalTS, inputData);

    //--- Custom Diffuse Start ---//

    half3 col = surfaceData.albedo;


    //--- Mandelbrot Start ---//

        float scale       = 1;
        int   iter        = 10;
        half ratio = 1.0;
        half t = _Time.y;

        scale             = scale * scale;

        half2 z = half2(0.0, 0.0);

        half zoomTime = 1.0;
        half2  uv       = input.uv;// - 0.5;
        // half2  c = (uv * half2(ratio, 1.0)) / (pow(zoomTime, zoomTime / 10) / 100) +
        //         half2(-0.746, 0.1481643);
        half2 c = half2(1.0, 1.0);

        int i;

        for (i = 1; i <= iter; i++)
        {
            float x = (z.x * z.x - z.y * z.y) + c.x;
            float y = (z.y * z.x + z.x * z.y) + c.y;

            if ((x * x + y * y) > scale)
                break;
            z.x = x;
            z.y = y;
        }

        // logInterp(z, float(i), float(iter))
        // sqrt(z.x * z.x + z.y * z.y)logInterp(z, float(i), float(iter))


        // col = to3((i >= iter)
        //                 ? to3(0.0)
        //                 // : to3(half3(0.0, 0.0, 0.0),
        //                 //       to3(
        //                 //           abs(sin(3.0 * t / 10.0)) / 100.0,
        //                 //           abs(cos(5.0 * t / 10.0)) / 100.0,
        //                 //            abs(sin(7.0 * t / 10.0)) / 100.0),
        //                 //       to3(logInterp(z, float(i), float(iter)))),
        //                 : hsv2rgb(half3(t * -0.1, 0.0, 0.0) +
        //                         lerp(half3(0.0, 1.0, 1.0), half3(0.01, 1.0, 1.0),
        //                             to3(logInterp(z, float(i), float(iter))))),
        //             1.0);

        col = half3(uv.x, 0.0, uv.y);

    //--- Mandelbrot End ---//
    

    surfaceData.albedo = col;

    //--- Custom Diffuse End ---//

    half4 color = UniversalFragmentPBR(inputData, surfaceData);


    color.rgb = MixFog(color.rgb, inputData.fogCoord);
    color.a = OutputAlpha(color.a, _Surface);


    return color;
}

#endif