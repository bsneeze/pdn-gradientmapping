using System;
using System.Collections.Generic;
using System.Text;
using PaintDotNet;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace pyrochild.effects.common
{
    [Serializable]
    public abstract class UnaryPixelHistogramOp : UnaryPixelOp
    {
        public abstract long[][] Apply(long[][] histogram);
    }

    public sealed class UnaryPixelHistogramOps
    {
        private UnaryPixelHistogramOps() { }

        [Serializable]
        public class LuminosityCurve
            : UnaryPixelHistogramOp
        {
            public byte[] Curve = new byte[256];

            public LuminosityCurve()
            {
            }

            public override ColorBgra Apply(ColorBgra color)
            {
                byte lumi = color.GetIntensityByte();
                int diff = Curve[lumi] - lumi;

                return ColorBgra.FromBgraClamped(
                    color.B + diff,
                    color.G + diff,
                    color.R + diff,
                    color.A);
            }

            public override long[][] Apply(long[][] histogram)
            {
                long[] before = histogram[0];
                long[] after = new long[256];
                for (int i = 0; i < before.Length; i++)
                {
                    after[Curve[i]] += before[i];
                }
                return new long[][] { after };
            }
        }

        [Serializable]
        public class RgbCurve
            : UnaryPixelHistogramOp
        {
            public byte[] CurveB = new byte[256];
            public byte[] CurveG = new byte[256];
            public byte[] CurveR = new byte[256];

            public RgbCurve()
            {
            }

            public override ColorBgra Apply(ColorBgra color)
            {
                return ColorBgra.FromBgra(CurveB[color.B], CurveG[color.G], CurveR[color.R], color.A);
            }

            public override long[][] Apply(long[][] histogram)
            {
                long[] beforeR = histogram[0];
                long[] beforeG = histogram[1];
                long[] beforeB = histogram[2];
                long[] afterR = new long[256];
                long[] afterG = new long[256];
                long[] afterB = new long[256];
                for (int i = 0; i < beforeR.Length; i++)
                {
                    afterR[CurveR[i]] += beforeR[i];
                    afterG[CurveG[i]] += beforeG[i];
                    afterB[CurveB[i]] += beforeB[i];
                }
                return new long[][] { afterR, afterG, afterB };
            }
        }

        [Serializable]
        public class AlphaCurve
            : UnaryPixelHistogramOp
        {
            public byte[] Curve = new byte[256];

            public AlphaCurve()
            {
            }

            public override ColorBgra Apply(ColorBgra color)
            {
                return ColorBgra.FromBgra(color.B, color.G, color.R, Curve[color.A]);
            }

            public override long[][] Apply(long[][] histogram)
            {
                long[] before = histogram[0];
                long[] after = new long[256];
                for (int i = 0; i < before.Length; i++)
                {
                    after[Curve[i]] += before[i];
                }
                return new long[][] { after };
            }
        }

        [Serializable]
        public class CmykCurve
            : UnaryPixelHistogramOp
        {
            public byte[] CurveC = new byte[256];
            public byte[] CurveM = new byte[256];
            public byte[] CurveY = new byte[256];
            public byte[] CurveK = new byte[256];

            public CmykCurve()
            {
            }

            public override ColorBgra Apply(ColorBgra color)
            {
                //find CMY
                byte C = (byte)(255 - color.R);
                byte M = (byte)(255 - color.G);
                byte Y = (byte)(255 - color.B);
                //find K
                byte K = (byte)Math.Min(Math.Min(C, M), Y);
                //convert to CMYK
                C -= K;
                M -= K;
                Y -= K;
                //curve
                C = CurveC[C];
                M = CurveM[M];
                Y = CurveY[Y];
                K = CurveK[K];
                //convert to CMY
                C = (C + K).ClampToByte();
                M = (M + K).ClampToByte();
                Y = (Y + K).ClampToByte();
                //return the RGB
                return ColorBgra.FromBgra((byte)(255 - Y), (byte)(255 - M), (byte)(255 - C), color.A);
            }

            public override long[][] Apply(long[][] histogram)
            {
                long[] beforeC = histogram[0];
                long[] beforeM = histogram[1];
                long[] beforeY = histogram[2];
                long[] beforeK = histogram[3];
                long[] afterC = new long[256];
                long[] afterM = new long[256];
                long[] afterY = new long[256];
                long[] afterK = new long[256];
                for (int i = 0; i < beforeC.Length; i++)
                {
                    afterC[CurveC[i]] += beforeC[i];
                    afterM[CurveM[i]] += beforeM[i];
                    afterY[CurveY[i]] += beforeY[i];
                    afterK[CurveK[i]] += beforeK[i];
                }
                return new long[][] { afterC, afterM, afterY, afterK };
            }
        }

        [Serializable]
        public class HsvCurve
            : UnaryPixelHistogramOp
        {
            public byte[] CurveH = new byte[256];
            public byte[] CurveS = new byte[256];
            public byte[] CurveV = new byte[256];

            public HsvCurve()
            {
            }

            public override ColorBgra Apply(ColorBgra color)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public override unsafe void Apply(ColorBgra* dst, ColorBgra* src, int length)
            {
                while (length > 0)
                {
                    int h = 0;
                    int s = 0;
                    int v = 0;

                    int max = (src->R > src->G) ? src->R : src->G;
                    if (max < src->B) max = src->B;

                    v = max;
                    int NewV = CurveV[v];

                    if (NewV == 0) // if the target Value is zero, there is no point calculating Saturation or Hue ...
                    {
                        dst->B = 0;
                        dst->G = 0;
                        dst->R = 0;
                    }
                    else
                    {
                        int min = (src->R < src->G) ? src->R : src->G;
                        if (min > src->B) min = src->B;
                        int delta = max - min;

                        if (delta > 0)
                        {
                            s = CommonUtil.IntDiv(255 * delta, max);
                        }

                        int NewS = CurveS[s];

                        if (NewS == 0) // if the target saturation is zero, then there is no point calculating a hue
                        {
                            dst->B = (byte)NewV;
                            dst->G = (byte)NewV;
                            dst->R = (byte)NewV;
                        }
                        else
                        {
                            if (src->R == max) // Between Yellow and Magenta
                            {
                                h = CommonUtil.IntDiv(255 * (src->G - src->B), delta);
                            }
                            else if (src->G == max) // Between Cyan and Yellow
                            {
                                h = 512 + CommonUtil.IntDiv(255 * (src->B - src->R), delta);
                            }
                            else // Between Magenta and Cyan
                            {
                                h = 1024 + CommonUtil.IntDiv(255 * (src->R - src->G), delta);
                            }

                            if (h < 0)
                            {
                                h += 1536;
                            }

                            h = CommonUtil.IntDiv(h, 6);
                        }
                        int NewH = CurveH[h];

                        if ((NewH == h) && (NewS == s))
                        {
                            dst->B = (byte)(CommonUtil.IntDiv(src->B * NewV, v));
                            dst->G = (byte)(CommonUtil.IntDiv(src->G * NewV, v));
                            dst->R = (byte)(CommonUtil.IntDiv(src->R * NewV, v));
                        }
                        else
                        {
                            NewH *= 6;
                            int fractionalSector = (NewH & 0xff);
                            int sectorNumber = (NewH >> 8);


                            dst->B = src->B;
                            dst->G = src->G;
                            dst->R = src->R;

                            //// Assign the fractional colors to r, g, and b
                            //// based on the sector the angle is in.
                            switch (sectorNumber)
                            {
                                case 0:
                                    dst->R = (byte)NewV;
                                    int tmp0 = ((NewS * (255 - fractionalSector)) + 128) >> 8;
                                    dst->G = (byte)(((NewV * (255 - tmp0)) + 128) >> 8);
                                    dst->B = (byte)(((NewV * (255 - NewS)) + 128) >> 8);
                                    break;

                                case 1:
                                    int tmp1 = ((NewS * fractionalSector) + 128) >> 8;
                                    dst->R = (byte)(((NewV * (255 - tmp1)) + 128) >> 8);
                                    dst->G = (byte)NewV;
                                    dst->B = (byte)(((NewV * (255 - NewS)) + 128) >> 8);
                                    break;

                                case 2:
                                    dst->R = (byte)(((NewV * (255 - NewS)) + 128) >> 8);
                                    dst->G = (byte)NewV;
                                    int tmp2 = ((NewS * (255 - fractionalSector)) + 128) >> 8;
                                    dst->B = (byte)(((NewV * (255 - tmp2)) + 128) >> 8);
                                    break;

                                case 3:
                                    dst->R = (byte)(((NewV * (255 - NewS)) + 128) >> 8);
                                    int tmp3 = ((NewS * fractionalSector) + 128) >> 8;
                                    dst->G = (byte)(((NewV * (255 - tmp3)) + 128) >> 8);
                                    dst->B = (byte)NewV;
                                    break;

                                case 4:
                                    int tmp4 = ((NewS * (255 - fractionalSector)) + 128) >> 8;
                                    dst->R = (byte)(((NewV * (255 - tmp4)) + 128) >> 8);
                                    dst->G = (byte)(((NewV * (255 - NewS)) + 128) >> 8);
                                    dst->B = (byte)NewV;
                                    break;

                                case 5:
                                    dst->R = (byte)NewV;
                                    dst->G = (byte)(((NewV * (255 - NewS)) + 128) >> 8);
                                    int tmp5 = ((NewS * fractionalSector) + 128) >> 8;
                                    dst->B = (byte)(((NewV * (255 - tmp5)) + 128) >> 8);
                                    break;
                            }
                        }
                    }

                    dst->A = src->A;

                    ++dst;
                    ++src;
                    --length;
                }
            }

            public override long[][] Apply(long[][] histogram)
            {
                long[] beforeH = histogram[0];
                long[] beforeS = histogram[1];
                long[] beforeV = histogram[2];
                long[] afterH = new long[256];
                long[] afterS = new long[256];
                long[] afterV = new long[256];
                for (int i = 0; i < beforeH.Length; i++)
                {
                    afterH[CurveH[i]] += beforeH[i];
                    afterS[CurveS[i]] += beforeS[i];
                    afterV[CurveV[i]] += beforeV[i];
                }
                return new long[][] { afterH, afterS, afterV };
            }
        }

        [Serializable]
        public class AdvancedCurve
            : UnaryPixelHistogramOp
        {
            public byte[] Curve = new byte[256];
            Channel InputMode;
            Channel OutputMode;

            public AdvancedCurve(Channel input, Channel output)
            {
                InputMode = input;
                OutputMode = output;
            }

            public override ColorBgra Apply(ColorBgra color)
            {
                int i = 0;
                int h, s, v, delta;
                switch (InputMode)
                {
                    case Channel.A:
                        i = Curve[color.A];
                        break;

                    case Channel.R:
                        i = Curve[color.R];
                        break;

                    case Channel.G:
                        i = Curve[color.G];
                        break;

                    case Channel.B:
                        i = Curve[color.B];
                        break;

                    case Channel.C:
                        i = 255 - color.R;
                        i -= Math.Min(Math.Min(255 - color.R, 255 - color.G), 255 - color.B);
                        i = Curve[i];
                        break;

                    case Channel.M:
                        i = 255 - color.G;
                        i -= Math.Min(Math.Min(255 - color.R, 255 - color.G), 255 - color.B);
                        i = Curve[i];
                        break;

                    case Channel.Y:
                        i = 255 - color.B;
                        i -= Math.Min(Math.Min(255 - color.R, 255 - color.G), 255 - color.B);
                        i = Curve[i];
                        break;

                    case Channel.K:
                        i = Math.Min(Math.Min(255 - color.R, 255 - color.G), 255 - color.B);
                        i = Curve[i];
                        break;

                    case Channel.H:
                        s = 0;
                        v = (color.R > color.G) ? color.R : color.G;
                        delta = 0;
                        if (color.B > v) v = color.B;
                        if (v == 0) s = 0;
                        else
                        {
                            int min = (color.R < color.G) ? color.R : color.G;
                            if (color.B < min) min = color.B;
                            delta = v - min;
                            if (delta > 0) s = CommonUtil.IntDiv(255 * delta, v);
                        }
                        if (s == 0) i = 0;
                        else
                        {
                            if (color.R == v) // Between Yellow and Magenta
                            {
                                i = CommonUtil.IntDiv(255 * (color.G - color.B), delta);
                            }
                            else if (color.G == v) // Between Cyan and Yellow
                            {
                                i = 512 + CommonUtil.IntDiv(255 * (color.B - color.R), delta);
                            }
                            else // Between Magenta and Cyan
                            {
                                i = 1024 + CommonUtil.IntDiv(255 * (color.R - color.G), delta);
                            }

                            if (i < 0)
                            {
                                i += 1536;
                            }

                            i = CommonUtil.IntDiv(i, 6);
                        }
                        i = Curve[i];
                        break;

                    case Channel.S:
                        v = (color.R > color.G) ? color.R : color.G;
                        if (color.B > v) v = color.B;
                        if (v == 0) i = 0;
                        else
                        {
                            int min = (color.R < color.G) ? color.R : color.G;
                            if (color.B < min) min = color.B;
                            delta = v - min;
                            if (delta > 0) i = CommonUtil.IntDiv(255 * delta, v);
                        }
                        i = Curve[i];
                        break;

                    case Channel.V:
                        i = (color.R > color.G) ? color.R : color.G;
                        if (color.B > i) i = color.B;
                        i = Curve[i];
                        break;

                    case Channel.L:
                        i = color.GetIntensityByte();
                        i = Curve[i];
                        break;

                    default: throw new Exception();
                }
                switch (OutputMode)
                {
                    case Channel.A:
                        return ColorBgra.FromBgra(
                            color.B,
                            color.G,
                            color.R,
                            (byte)i);

                    case Channel.R:
                        return ColorBgra.FromBgra(
                            color.B,
                            color.G,
                            (byte)i,
                            color.A);

                    case Channel.G:
                        return ColorBgra.FromBgra(
                            color.B,
                            (byte)i,
                            color.R,
                            color.A);

                    case Channel.B:
                        return ColorBgra.FromBgra(
                            (byte)i,
                            color.G,
                            color.R,
                            color.A);

                    case Channel.C:
                        i = (byte)(i + Math.Min(Math.Min(255 - color.R, 255 - color.G), 255 - color.B)).Clamp(0,255);
                        return ColorBgra.FromBgra(
                            color.B,
                            color.G,
                            (byte)(255 - i),
                            color.A);

                    case Channel.M:
                        i = (byte)(i + Math.Min(Math.Min(255 - color.R, 255 - color.G), 255 - color.B)).Clamp(0,255);
                        return ColorBgra.FromBgra(
                            color.B,
                            (byte)(255 - i),
                            color.R,
                            color.A);

                    case Channel.Y:
                        i = (byte)(i + Math.Min(Math.Min(255 - color.R, 255 - color.G), 255 - color.B)).Clamp(0,255);
                        return ColorBgra.FromBgra(
                            (byte)(255 - i),
                            color.G,
                            color.R,
                            color.A);

                    case Channel.K:
                        int C = 255 - color.R;
                        int M = 255 - color.G;
                        int Y = 255 - color.B;
                        int K = Math.Min(Math.Min(C, M), Y);
                        return ColorBgra.FromBgraClamped(
                            255 - (Y - K + i),
                            255 - (M - K + i),
                            255 - (C - K + i),
                            color.A);

                    case Channel.H:
                        s = 0;
                        v = (color.R > color.G) ? color.R : color.G;
                        delta = 0;
                        if (color.B > v) v = color.B;
                        if (v == 0) s = 0;
                        else
                        {
                            int min = (color.R < color.G) ? color.R : color.G;
                            if (color.B < min) min = color.B;
                            delta = v - min;
                            if (delta > 0) s = CommonUtil.IntDiv(255 * delta, v);
                        }
                        i *= 6;
                        int fSector = (i & 0xff);
                        int sNumber = (i >> 8);
                        switch (sNumber)
                        {
                            case 0:
                                int tmp0 = ((s * (255 - fSector)) + 128) >> 8;
                                return ColorBgra.FromBgra(
                                    (byte)(((v * (255 - s)) + 128) >> 8),
                                    (byte)(((v * (255 - tmp0)) + 128) >> 8),
                                    (byte)v,
                                    color.A);
                            case 1:
                                int tmp1 = ((s * fSector) + 128) >> 8;
                                return ColorBgra.FromBgra(
                                    (byte)(((v * (255 - s)) + 128) >> 8),
                                    (byte)v,
                                    (byte)(((v * (255 - tmp1)) + 128) >> 8),
                                    color.A);
                            case 2:
                                int tmp2 = ((s * (255 - fSector)) + 128) >> 8;
                                return ColorBgra.FromBgra(
                                    (byte)(((v * (255 - tmp2)) + 128) >> 8),
                                    (byte)v,
                                    (byte)(((v * (255 - s)) + 128) >> 8),
                                    color.A);
                            case 3:
                                int tmp3 = ((s * fSector) + 128) >> 8;
                                return ColorBgra.FromBgra(
                                    (byte)v,
                                    (byte)(((v * (255 - tmp3)) + 128) >> 8),
                                    (byte)(((v * (255 - s)) + 128) >> 8),
                                    color.A);
                            case 4:
                                int tmp4 = ((s * (255 - fSector)) + 128) >> 8;
                                return ColorBgra.FromBgra(
                                    (byte)v,
                                    (byte)(((v * (255 - s)) + 128) >> 8),
                                    (byte)(((v * (255 - tmp4)) + 128) >> 8),
                                    color.A);
                            case 5:
                                int tmp5 = ((s * fSector) + 128) >> 8;
                                return ColorBgra.FromBgra(
                                    (byte)(((v * (255 - tmp5)) + 128) >> 8),
                                    (byte)(((v * (255 - s)) + 128) >> 8),
                                    (byte)v,
                                    color.A);
                            default:
                                return new ColorBgra();
                        }

                    case Channel.S:
                        s = 0;
                        v = (color.R > color.G) ? color.R : color.G;
                        delta = 0;
                        if (color.B > v) v = color.B;
                        if (v == 0) s = 0;
                        else
                        {
                            int min = (color.R < color.G) ? color.R : color.G;
                            if (color.B < min) min = color.B;
                            delta = v - min;
                            if (delta > 0) s = CommonUtil.IntDiv(255 * delta, v);
                        }
                        if (s == 0) h = 0;
                        else
                        {
                            if (color.R == v) // Between Yellow and Magenta
                            {
                                h = CommonUtil.IntDiv(255 * (color.G - color.B), delta);
                            }
                            else if (color.G == v) // Between Cyan and Yellow
                            {
                                h = 512 + CommonUtil.IntDiv(255 * (color.B - color.R), delta);
                            }
                            else // Between Magenta and Cyan
                            {
                                h = 1024 + CommonUtil.IntDiv(255 * (color.R - color.G), delta);
                            }

                            if (h < 0)
                            {
                                h += 1536;
                            }
                        }

                        int fs = (h & 0xff);
                        int sn = (h >> 8);
                        switch (sn)
                        {
                            case 0:
                                int tmp0 = ((i * (255 - fs)) + 128) >> 8;
                                return ColorBgra.FromBgra(
                                    (byte)(((v * (255 - i)) + 128) >> 8),
                                    (byte)(((v * (255 - tmp0)) + 128) >> 8),
                                    (byte)v,
                                    color.A);
                            case 1:
                                int tmp1 = ((i * fs) + 128) >> 8;
                                return ColorBgra.FromBgra(
                                    (byte)(((v * (255 - i)) + 128) >> 8),
                                    (byte)v,
                                    (byte)(((v * (255 - tmp1)) + 128) >> 8),
                                    color.A);
                            case 2:
                                int tmp2 = ((i * (255 - fs)) + 128) >> 8;
                                return ColorBgra.FromBgra(
                                    (byte)(((v * (255 - tmp2)) + 128) >> 8),
                                    (byte)v,
                                    (byte)(((v * (255 - i)) + 128) >> 8),
                                    color.A);
                            case 3:
                                int tmp3 = ((i * fs) + 128) >> 8;
                                return ColorBgra.FromBgra(
                                    (byte)v,
                                    (byte)(((v * (255 - tmp3)) + 128) >> 8),
                                    (byte)(((v * (255 - i)) + 128) >> 8),
                                    color.A);
                            case 4:
                                int tmp4 = ((i * (255 - fs)) + 128) >> 8;
                                return ColorBgra.FromBgra(
                                    (byte)v,
                                    (byte)(((v * (255 - i)) + 128) >> 8),
                                    (byte)(((v * (255 - tmp4)) + 128) >> 8),
                                    color.A);
                            case 5:
                                int tmp5 = ((i * fs) + 128) >> 8;
                                return ColorBgra.FromBgra(
                                    (byte)(((v * (255 - tmp5)) + 128) >> 8),
                                    (byte)(((v * (255 - i)) + 128) >> 8),
                                    (byte)v,
                                    color.A);
                            default:
                                return new ColorBgra();
                        }

                    case Channel.V:
                        int max = (color.R > color.G) ? color.R : color.G;
                        if (color.B > max) max = color.B;
                        return ColorBgra.FromBgra(
                            (byte)CommonUtil.IntDiv(color.B * i, max),
                            (byte)CommonUtil.IntDiv(color.G * i, max),
                            (byte)CommonUtil.IntDiv(color.R * i, max),
                            color.A
                            );

                    case Channel.L:
                        return ColorBgra.FromBgraClamped(
                            color.B + i - color.GetIntensityByte(),
                            color.G + i - color.GetIntensityByte(),
                            color.R + i - color.GetIntensityByte(),
                            color.A);
                    default:
                        throw new Exception();
                }
            }

            public override long[][] Apply(long[][] histogram)
            {
                return null;
            }
        }
        
        [Serializable]
        public class GradientMap : UnaryPixelOp
        {
            private ColorBgra[] lookup;
            Channel input;
            private bool lockalpha;
            private bool wrap;
            private int offset;

            public GradientMap(Gradient gradient, Channel input, bool wrap, bool lockalpha, int offset)
            {
                this.input = input;
                this.wrap = wrap;
                this.offset = offset;
                this.lockalpha = lockalpha;
                if (gradient != null && gradient.Count > 0)
                {
                    lookup = new ColorBgra[256];
                    for (int i = 0; i < 256; i++)
                    {
                        float f = i / 255f;

                        lookup[i] = gradient.GetColor(f);
                    }
                }
            }

            public override ColorBgra Apply(ColorBgra color)
            {
                ColorPlus c = new ColorPlus(color);
                byte b = 0;
                switch (input)
                {
                    case Channel.A:
                        b = c.A;
                        break;
                    case Channel.R:
                        b = c.R;
                        break;
                    case Channel.G:
                        b = c.G;
                        break;
                    case Channel.B:
                        b = c.B;
                        break;
                    case Channel.C:
                        b = c.C;
                        break;
                    case Channel.M:
                        b = c.M;
                        break;
                    case Channel.Y:
                        b = c.Y;
                        break;
                    case Channel.K:
                        b = c.K;
                        break;
                    case Channel.H:
                        b = c.H;
                        break;
                    case Channel.S:
                        b = c.S;
                        break;
                    case Channel.V:
                        b = c.V;
                        break;
                    case Channel.L:
                        b = c.L;
                        break;
                }
                if (wrap)
                {
                    b = (byte)(b + offset);
                }
                else
                {
                    b = (b + offset).ClampToByte();
                }
                if (!lockalpha)
                {
                    return lookup[b];
                }
                else
                {
                    return lookup[b].NewAlpha(color.A);
                }
            }
        }
    }
}