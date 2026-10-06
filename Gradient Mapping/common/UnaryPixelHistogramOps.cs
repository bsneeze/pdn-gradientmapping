using System;
using System.Collections.Generic;
using System.Text;
using PaintDotNet;
using PaintDotNet.Imaging;
using PaintDotNet.Rendering;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace pyrochild.effects.common
{
    public static class UnaryPixelOpExtensions
    {
        public static unsafe void Apply(this UnaryPixelOp op, RegionPtr<ColorBgra32> dst, RegionPtr<ColorBgra32> src, RectInt32 tileBounds)
        {
            RegionPtr<ColorBgra> dstBgra = dst.Cast<ColorBgra>();
            RegionPtr<ColorBgra> srcBgra = src.Cast<ColorBgra>();

            for (int y = 0; y < dstBgra.Height; ++y)
            {
                ColorBgra* srcRow = srcBgra.Rows[tileBounds.Y + y].Ptr + tileBounds.X;
                op.Apply(dstBgra.Rows[y].Ptr, srcRow, dstBgra.Width);
            }
        }
    }

    public sealed class UnaryPixelHistogramOps
    {
        private UnaryPixelHistogramOps() { }

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
                int max = (color.R > color.G) ? color.R : color.G;
                if (max < color.B) max = color.B;
                int min = (color.R < color.G) ? color.R : color.G;
                if (min > color.B) min = color.B;
                int delta = max - min;

                byte b = 0;
                switch (input)
                {
                    case Channel.A:
                        b = color.A;
                        break;
                    case Channel.R:
                        b = color.R;
                        break;
                    case Channel.G:
                        b = color.G;
                        break;
                    case Channel.B:
                        b = color.B;
                        break;
                    case Channel.C:
                        b = (byte)(max - color.R);
                        break;
                    case Channel.M:
                        b = (byte)(max - color.G);
                        break;
                    case Channel.Y:
                        b = (byte)(max - color.B);
                        break;
                    case Channel.K:
                        b = (byte)(255 - max);
                        break;
                    case Channel.H:
                        if (delta != 0)
                        {
                            int h;
                            if (color.R == max) // Between Yellow and Magenta
                            {
                                h = CommonUtil.IntDiv(255 * (color.G - color.B), delta);
                            }
                            else if (color.G == max) // Between Cyan and Yellow
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

                            b = (byte)CommonUtil.IntDiv(h, 6);
                        }
                        break;
                    case Channel.S:
                        if (delta != 0)
                        {
                            b = (byte)CommonUtil.IntDiv(255 * delta, max);
                        }
                        break;
                    case Channel.V:
                        b = (byte)max;
                        break;
                    case Channel.L:
                        b = (byte)((7471 * color.B + 38470 * color.G + 19595 * color.R) >> 16);
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
