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

using System;

namespace pyrochild.effects.common
{
    internal static class SrgbGamma
    {
        private static double[] linearIntensity = new double[0x100];

        static SrgbGamma()
        {
            for (int i = 0; i <= 0xff; i++)
            {
                double srgbLevel = ((double)i) / 255.0;
                linearIntensity[i] = ToLinear(srgbLevel);
            }
        }

        public static double ToLinear(byte srgbLevel)
        {
            return linearIntensity[srgbLevel];
        }

        public static double ToLinear(double srgbLevel)
        {
            if (srgbLevel <= 0.04045)
            {
                return (srgbLevel * 0.077399380804953566);
            }
            return Math.Pow((srgbLevel + 0.055) * 0.94786729857819907, 2.4);
        }

        public static double ToSrgb(double linearLevel)
        {
            if (linearLevel <= 0.0031308)
            {
                return (12.92 * linearLevel);
            }
            return ((1.055 * Math.Pow(linearLevel, 0.41666666666666669)) - 0.055);
        }

        public static double ToSrgbClamped(double linearLevel)
        {
            return ((linearLevel < 0.0) ? 0.0 : ((linearLevel > 1.0) ? 1.0 : ToSrgb(linearLevel)));
        }
    }
}