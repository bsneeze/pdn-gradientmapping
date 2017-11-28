using PaintDotNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace pyrochild.effects.common
{
    [Serializable]
    public class Gradient : ICloneable
    {
        public struct GradientColor : IComparable<GradientColor>
        {
            public double Position;
            public ColorBgra Color;

            public GradientColor(double position, ColorBgra color)
            {
                Position = position;
                Color = color;
            }

            public int CompareTo(GradientColor other)
            {
                if (Position < other.Position) return -1;
                if (Position > other.Position) return 1;
                return 0;

            }
        }

        private List<GradientColor> colors;

        public Gradient()
        {
            colors = new List<GradientColor>();
        }

        public ColorBgra[] Colors
        {
            get
            {
                var retval = new ColorBgra[Count];
                for (int i = 0; i < Count; i++)
                {
                    retval[i] = GetColor(i);
                }
                return retval;
            }
            set
            {
                double space = value.Length > 1 ? 1.0 / (value.Length - 1) : 0;

                for (int i = 0; i < value.Length; i++)
                {
                    if (i < Count)
                    {
                        SetColor(i, value[i]);
                        SetPosition(i, i * space);
                    }
                    else
                    {
                        Add(i * space, value[i]);
                    }
                }
            }
        }

        public double[] Positions
        {
            get
            {
                var retval = new double[Count];
                for (int i = 0; i < Count; i++)
                {
                    retval[i] = GetPosition(i);
                }
                return retval;
            }
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (i < Count)
                    {
                        SetPosition(i, value[i]);
                    }
                    else
                    {
                        Add(value[i], ColorBgra.Black);
                    }
                }
            }
        }

        public int Count
        {
            get
            {
                return colors.Count;
            }
        }
        
        public void Reverse()
        {
            colors.ForEach(color => color.Position = 1 - color.Position);
            colors.Reverse();
        }

        public void Add(double position, ColorBgra color)
        {
            colors.Add(new GradientColor(position, color));
            colors.Sort();
        }

        public void RemoveAt(int index)
        {
            colors.RemoveAt(index);
        }

        /// <summary>
        /// returns the blended color from the gradient
        /// </summary>
        /// <param name="position">position to get color from [0,1]</param>
        /// <returns>the color...</returns>
        public ColorBgra GetColor(double position)
        {
            if (Count > 0)
            {
                int index1 = 0, index2 = colors.Count - 1;
                for (int i = 0; i < Count; i++)
                {
                    if ((colors[i].Position <= position) && (colors[i].Position >= colors[index1].Position))
                    {
                        index1 = i;
                    }
                    if ((colors[i].Position >= position) && (colors[i].Position <= colors[index2].Position))
                    {
                        index2 = i;
                    }
                }
                if (colors[index1].Position == colors[index2].Position)
                {
                    return colors[index1].Color;
                }

                return ColorBgraBlender.Blend(colors[index1].Color, colors[index2].Color,
                    (position - colors[index1].Position) / (colors[index2].Position - colors[index1].Position));
            }
            else
            {
                throw new InvalidOperationException("Gradient contains no colors.");
            }
        }

        public object Clone()
        {
            Gradient retval = new Gradient();
            retval.colors.AddRange(colors);
            return retval;
        }

        /// <summary>
        /// Evenly spaces all of the colors.
        /// </summary>
        public void Spread()
        {
            if (Count <= 1)
                return;

            double space = 1.0 / (Count - 1);

            for (int i = 0; i < Count; i++)
            {
                colors[i] = new GradientColor(i * space, colors[i].Color);
            }
        }

        public void Clear()
        {
            colors.Clear();
        }

        public void SetDefault()
        {
            Clear();
            colors.AddRange(new GradientColor[] {
                new GradientColor(0, ColorBgra.Black),
                new GradientColor(1, ColorBgra.White)
            });
        }

        /// <summary>
        /// Moves the gradient control point to the given position
        /// </summary>
        /// <param name="index">Index of the control point</param>
        /// <param name="position">Position to move the control point to</param>
        /// <returns>The new index of the control point that was moved</returns>
        public int SetPosition(int index, double position)
        {
            GradientColor gc = colors[index];
            gc.Position = position;
            colors[index] = gc;

            while (index + 1 < Count && position > colors[index + 1].Position)
            {
                GradientColor t = colors[index];
                colors[index] = colors[index + 1];
                colors[index + 1] = t;
                index++;
            }

            while (index - 1 >= 0 && position < colors[index - 1].Position)
            {
                GradientColor t = colors[index];
                colors[index] = colors[index - 1];
                colors[index - 1] = t;
                index--;
            }

            return index;
        }

        public double GetPosition(int index)
        {
            return colors[index].Position;
        }

        public ColorBgra GetColor(int index)
        {
            return colors[index].Color;
        }

        public void SetColor(int index, ColorBgra color)
        {
            if (index >= 0 && index < Count)
            {
                GradientColor gc = colors[index];
                gc.Color = color;
                colors[index] = gc;
            }
        }

        public void DrawToGraphics(Graphics g, Rectangle bounds)
        {
            if (Count > 0)
            {
                using (Brush brush = new HatchBrush(HatchStyle.LargeCheckerBoard, Color.DarkGray, Color.White))
                {
                    g.FillRectangle(brush, bounds);
                }
                g.SmoothingMode = SmoothingMode.None;
                using (Pen pen = new Pen(Color.Black, 1f))
                {
                    int width = bounds.Width;
                    int left = bounds.Left;
                    for (int i = 0; i <= width; i++)
                    {
                        double position = ((double)i) / ((double)width);
                        pen.Color = GetColor(position).ToColor();
                        g.DrawLine(pen, left, bounds.Top, left, bounds.Bottom);
                        left++;
                    }
                }
            }
        }
    }
}