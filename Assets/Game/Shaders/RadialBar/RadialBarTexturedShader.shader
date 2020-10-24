Shader "RadialBarTextured" {
    Properties {

         [NoScaleOffset] _MainTex ("MainTex", 2D) = "White" {}

        [Toggle] _UseTexture ("UseTexture", Float) = 1.0

        _Frac ("Progress Bar Value", Range(0,1)) = 1.0
        _FillColor ("Fill Color", Color) = (1,1,1,1)
        _BackColor ("Background Color", Color) = (0,0,0,1)

        [Header(Ring)]
        _OuterRadius ("Outer Radius", Range(0,1)) = 0.95
        _InnerRadius ("Inner Radius", Range(0,1)) = 0.5

        [Header(Outline)]
        [Toggle(USE_OUTLINE)] _UseOutline ("Enable", Float) = 0.0
        _OutlineColor ("Color", Color) = (0,0,0,1)
        _OutlineWidth ("Width", Range(0,1)) = 0.1

        [Header(Arc)]
        [Toggle(USE_ARC)] _UseArc ("Enable", Float) = 0.0
        _ArcAngle ("Angle", Range(-180,180)) = 0.0
        _ArcRange ("Range", Range(0,1)) = 0.75

        [Space(10)]
        [Toggle] _FlipU ("Flip Horizontal", Float) = 0.0
        [Toggle] _FlipV ("Flip Vertical", Float) = 0.0

    }

    SubShader {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "DisableBatching"="True"}

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature USE_OUTLINE
            #pragma shader_feature USE_ARC

            #include "UnityCG.cginc"

            // Direct3D compiled stats:
            // vertex shader:
            //   11 math w/o arc
            //   20 math w/ arc
            // fragment shader:
            //   28 math w/o arc or outline
            //   35 math w/ arc
            //   37 math w/ outline
            //   48 math w/ arc and outline

            // texture we will sample
            sampler2D _MainTex;

            half _Frac;
            fixed4 _FillColor;
            fixed4 _BackColor;

            fixed4 _OutlineColor;

            half _OuterRadius;
            half _InnerRadius;

            half _OutlineWidth;

            bool _UseTexture;
            bool _FlipU;
            bool _FlipV;

            half _ArcAngle;
            half _ArcRange;

            struct v2f {
                float2 uv : TEXCOORD4;
                float4 pos : SV_POSITION;
            #if defined(USE_ARC)
                float4 uvMask : TEXCOORD0;
            #else
                float3 uvMask : TEXCOORD0;
            #endif
            };

            v2f vert (appdata_img v, float2 uv : TEXCOORD4)
            {
                v2f o;

                o.uv = uv;
                o.pos = UnityObjectToClipPos(v.vertex);

                o.uvMask.xy = v.texcoord.xy * 2.0 - 1.0;

                // options for flipping the direction
                if (_FlipU)
                    o.uvMask.x = -o.uvMask.x;
                if (_FlipV)
                    o.uvMask.y = -o.uvMask.y;

                float barFrac = _Frac;
                float sinX, cosX;

            #if defined(USE_ARC)
                 // rotate base masks by arc angle
                float minAngle = _ArcAngle * (UNITY_PI / 180.0) + (1.0 - _ArcRange) * UNITY_PI;
                sincos(minAngle, sinX, cosX);
                float2x2 minRotationMatrix = float2x2(cosX, -sinX, sinX, cosX);
                o.uvMask.xy = mul(o.uvMask.xy, minRotationMatrix);

                // rotated mask for end of arc
                float maxAngle = _ArcRange * (UNITY_PI * 2.0);
                sincos(maxAngle, sinX, cosX);
                float2x2 maxRotationMatrix = float2x2(cosX, -sinX, sinX, cosX);
                o.uvMask.w = -mul(o.uvMask.xy, maxRotationMatrix).x;

                // scale progress bar value based on arc range
                barFrac *= _ArcRange;
            #endif // USE_ARC

                // angled mask for end of progress bar
                float angle = barFrac * (UNITY_PI * 2.0) - UNITY_PI;
                sincos(angle, sinX, cosX);
                float2x2 rotationMatrix = float2x2(cosX, -sinX, sinX, cosX);
                o.uvMask.z = mul(o.uvMask.xy, rotationMatrix).x;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half barFrac = _Frac;

                // radial gradient for circles
                half radialGrad = length(i.uvMask.xy);

                // accurate derivative length rather than fwidth
                half radialGradDeriv = length(half2(ddx(radialGrad), ddy(radialGrad))) * 0.75;

                // outer and inner circle masks for progress bar
                half outerEdge = _OuterRadius - radialGrad;
                half innerEdge = radialGrad - _InnerRadius;

                // progress bar circle edge mask
                half circleEdge = smoothstep(-radialGradDeriv, radialGradDeriv, min(outerEdge, innerEdge));

            #if defined(USE_OUTLINE)
                // outline circle edge mask
                half outlineEdge = max(smoothstep(-radialGradDeriv, radialGradDeriv, min(outerEdge, innerEdge) + _OutlineWidth), circleEdge);
            #endif // USE_OUTLINE

                // sharpen masks with screen space derivates
                half diag = i.uvMask.z / fwidth(i.uvMask.z);
                half vert = i.uvMask.x / fwidth(i.uvMask.x);

            #if defined(USE_ARC)
                // scale progress bar value based on arc range
                barFrac *= _ArcRange;

                // get arc end
                half arc_max_edge = i.uvMask.w / fwidth(i.uvMask.w) + 0.5;

                // init arc edges for outline
                half arc_outline_min = 0;
                half arc_outline_max = 0;

            #if defined(USE_OUTLINE)
                // set offset arc edges for outline
                arc_outline_min = (i.uvMask.x - _OutlineWidth) / fwidth(i.uvMask.x);
                arc_outline_max = (i.uvMask.w - _OutlineWidth) / fwidth(i.uvMask.w);
            #endif // USE_OUTLINE

                // arc masks for circle and outline edge mask
                half circleArcMask = 0;
                half outlineArcMask = 0;

                if ((_ArcRange) < 1.0)
                {
                    // "flip" arc mask depending on if less than 180 degrees
                    if ((_ArcRange) < 0.5)
                    {
                        // arc is wedge
                        circleArcMask = max(vert, arc_max_edge);
                        outlineArcMask = max(arc_outline_min, arc_outline_max);
                    }
                    else
                    {
                        // remove wedge
                        circleArcMask = min(vert, arc_max_edge);
                        outlineArcMask = min(arc_outline_min, arc_outline_max);
                    }

                    // cut out arc wedge from circle edge mask
                    circleEdge = min(circleEdge, 1.0 - saturate(circleArcMask));

                #if defined(USE_OUTLINE)
                    // cut out arc wedge from outline edge mask
                    outlineEdge = min(outlineEdge, 1.0 - saturate(outlineArcMask));
                #endif // USE_OUTLINE

                    // hack to prevent color bleed at the starting edge of the progress bar
                    vert -= 0.5;
                }
            #endif // USE_ARC

                // "flip" the masks depending on progress bar value
                half barProgress = 0.0;
                if (barFrac < 0.5)
                    barProgress = max(diag, vert);
                else
                    barProgress = min(diag, vert);

                // mask bottom of progress bar when below 20%
                if (barFrac < 0.2 && i.uvMask.y < 0.0)
                    barProgress = 1.0;

                barProgress = saturate(barProgress);

                // lerp between colors
                fixed4 col = lerp(_FillColor, _BackColor, barProgress);

            #if defined(USE_OUTLINE)
                // lerp to outline color if outline > 0.0
                if (_OutlineWidth > 0.0)
                    col = lerp(_OutlineColor, col, circleEdge);

                // apply outline mask as alpha
                col.a *= outlineEdge;
            #else // !defined(USE_OUTLINE)

                // apply circle mask as alpha
                col.a *= circleEdge;
            #endif // USE_OUTLINE
            fixed4 col2 = tex2D(_MainTex, i.uv);
            // col2.a = 1.0;
            if (col2.a < col.a) {
                col.a = col2.a;
            }
            // if (col.r == 0 && col.b ==0 && col.c == 0) {

            // }
            return col;
            // return col2;
            if (col2.a == 0) {
                col.a = 0;
                return col;
            }
            else {
                if (_UseTexture == 1.0) {
                    if (col.a < 1.0) {
                        return _BackColor;
                    }
                    else {
                        fixed4 col2 = tex2D(_MainTex, i.uv);
                        col2.a = col.a;
                        return col2;
                    }
                }
                else {
                    return col;
                }
            }
            }
            ENDCG
        }
    }
}