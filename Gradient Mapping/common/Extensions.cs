using PaintDotNet;
using PaintDotNet.Imaging;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;

namespace pyrochild.effects.common
{
    public static class Extensions
    {
        public static Color ToOpaqueColor(this Color color)
        {
            return Color.FromArgb(255, color);
        }

        public static byte ClampToByte(this int val)
        {
            if (val > 255) return 255;
            if (val < 0) return 0;
            return (byte)val;
        }

        public static byte ClampToByte(this double val)
        {
            if (val > 255) return 255;
            if (val < 0) return 0;
            return (byte)val;
        }

        public static byte ClampToByte(this float val)
        {
            if (val > 255) return 255;
            if (val < 0) return 0;
            return (byte)val;
        }

        public static string StripIllegalPathChars(this string str)
        {
            StringBuilder sb = new StringBuilder(str);
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                sb.Replace(c.ToString(), "");
            }
            return sb.ToString();
        }

        public static ColorBgra ToColorBgra(this ColorHsv96Float color)
        {
            ColorRgb96Float rgb = color.ToRgb();
            return ColorBgra.FromBgraClamped(rgb.B * 255f, rgb.G * 255f, rgb.R * 255f, 255f);
        }

        public static ColorHsv96Float ToHsvColor(this ColorBgra color)
        {
            ColorRgb96Float rgb = new ColorRgb96Float(color.R / 255f, color.G / 255f, color.B / 255f);
            return rgb.ToHsv();
        }
    }
}