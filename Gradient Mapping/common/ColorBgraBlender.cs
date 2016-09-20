/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

// Copyright (c) 2007, 2008 Ed Harvey 
//
// MIT License: http://www.opensource.org/licenses/mit-license.php
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions: 
//
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software. 
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE. 
//

using PaintDotNet;
using System;

namespace pyrochild.effects.common
{
    public static class ColorBgraBlender
    {
        private const double reciprical255 = 0.00392156862745098;
        private const double reciprical510 = 0.00196078431372549;
        private const double reciprical65025 = 1.5378700499807768E-05;

        public static unsafe ColorBgra Blend(ColorBgra[] colors)
        {
            fixed (ColorBgra* bgraRef = colors)
            {
                return Blend(bgraRef, colors.Length);
            }
        }

        public static unsafe ColorBgra Blend(ColorBgra[] colors, int count)
        {
            fixed (ColorBgra* bgraRef = colors)
            {
                return Blend(bgraRef, count);
            }
        }

        public static unsafe ColorBgra Blend(ColorBgra* colors, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count must be 0 or greater");
            }
            if (count == 0)
            {
                return ColorBgra.Transparent;
            }
            ulong num = 0L;
            double num2 = 0.0;
            for (int i = 0; i < count; i++)
            {
                num += colors[i].A;
                num2 += SrgbGamma.ToLinear(colors[i].A);
            }
            byte a = (byte)(0.5 + (((double)num) / ((double)count)));
            double num5 = num2 / ((double)count);
            if (a == 0)
            {
                return ColorBgra.Transparent;
            }
            double num6 = 0.0;
            double num7 = 0.0;
            double num8 = 0.0;
            for (int j = 0; j < count; j++)
            {
                double num10 = colors[j].A * 1.5378700499807768E-05;
                num7 += SrgbGamma.ToLinear((double)(num10 * colors[j].G));
                num8 += SrgbGamma.ToLinear((double)(num10 * colors[j].R));
            }
            double num11 = (255.0 * count) / ((double)num);
            byte b = (byte)(0.5 + ((255.0 * SrgbGamma.ToSrgbClamped(num6 / ((double)count))) * num11));
            byte g = (byte)(0.5 + ((255.0 * SrgbGamma.ToSrgbClamped(num7 / ((double)count))) * num11));
            byte r = (byte)(0.5 + ((255.0 * SrgbGamma.ToSrgbClamped(num8 / ((double)count))) * num11));
            return ColorBgra.FromBgra(b, g, r, a);
        }

        public static ColorBgra Blend(ColorBgra ca, ColorBgra cb, byte cbAlpha)
        {
            double num = ((0xff - ca.A) * cbAlpha) * 1.5378700499807768E-05;
            double num2 = (ca.B * cbAlpha) * 1.5378700499807768E-05;
            double num3 = num + num2;
            if (num3 < 0.00196078431372549)
            {
                return ColorBgra.Transparent;
            }
            double num4 = 1.0 / num3;
            double linearLevel = ((SrgbGamma.ToLinear(ca.R) * num) + (SrgbGamma.ToLinear(cb.R) * num2)) * num4;
            double num6 = ((SrgbGamma.ToLinear(ca.G) * num) + (SrgbGamma.ToLinear(cb.G) * num2)) * num4;
            double num7 = ((SrgbGamma.ToLinear(ca.B) * num) + (SrgbGamma.ToLinear(cb.B) * num2)) * num4;
            return ColorBgra.FromBgra((byte)(0.5 + (255.0 * SrgbGamma.ToSrgb(num7))), (byte)(0.5 + (255.0 * SrgbGamma.ToSrgb(num6))), (byte)(0.5 + (255.0 * SrgbGamma.ToSrgb(linearLevel))), (byte)(0.5 + (255.0 * num3)));
        }

        public static ColorBgra Blend(ColorBgra ca, ColorBgra cb, double blend)
        {
            double num = 1.0 - blend;
            double num2 = blend;
            double linearLevel = (SrgbGamma.ToLinear(ca.R) * num) + (SrgbGamma.ToLinear(cb.R) * num2);
            double num4 = (SrgbGamma.ToLinear(ca.G) * num) + (SrgbGamma.ToLinear(cb.G) * num2);
            double num5 = (SrgbGamma.ToLinear(ca.B) * num) + (SrgbGamma.ToLinear(cb.B) * num2);
            double num6 = (ca.A * num) + (cb.A * num2);
            return ColorBgra.FromBgra((byte)(0.5 + (255.0 * SrgbGamma.ToSrgbClamped(num5))), (byte)(0.5 + (255.0 * SrgbGamma.ToSrgbClamped(num4))), (byte)(0.5 + (255.0 * SrgbGamma.ToSrgbClamped(linearLevel))), (byte)(0.5 + num6));
        }
    }
}