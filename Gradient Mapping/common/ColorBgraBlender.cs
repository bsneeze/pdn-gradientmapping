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
        public static ColorBgra Blend(ColorBgra ca, ColorBgra cb, double blend)
        {
            double num6 = (ca.A * (1.0 - blend)) + (cb.A * blend);

            // weight each color by its alpha, so a transparent color doesn't tint the blend.
            // if both weights are zero there's no alpha to go by, so use the plain weights.
            double num = (1.0 - blend) * ca.A;
            double num2 = blend * cb.A;
            double total = num + num2;
            if (total > 0.0)
            {
                num /= total;
                num2 /= total;
            }
            else
            {
                num = 1.0 - blend;
                num2 = blend;
            }
            double linearLevel = (SrgbGamma.ToLinear(ca.R) * num) + (SrgbGamma.ToLinear(cb.R) * num2);
            double num4 = (SrgbGamma.ToLinear(ca.G) * num) + (SrgbGamma.ToLinear(cb.G) * num2);
            double num5 = (SrgbGamma.ToLinear(ca.B) * num) + (SrgbGamma.ToLinear(cb.B) * num2);
            return ColorBgra.FromBgra((byte)(0.5 + (255.0 * SrgbGamma.ToSrgbClamped(num5))), (byte)(0.5 + (255.0 * SrgbGamma.ToSrgbClamped(num4))), (byte)(0.5 + (255.0 * SrgbGamma.ToSrgbClamped(linearLevel))), (byte)(0.5 + num6));
        }
    }
}