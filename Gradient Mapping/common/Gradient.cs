using PaintDotNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace pyrochild.effects.common
{
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

        public int Count
        {
            get
            {
                return colors.Count;
            }
        }
        
        public void Reverse()
        {
            for(int i = 0; i < Count; i++)
            {
                GradientColor gc = colors[i];
                gc.Position = 1 - gc.Position;
                colors[i] = gc;
            }
            colors.Reverse();
        }

        public void Add(double position, ColorBgra color)
        {
            // insert after any colors already at this position. List.Sort is unstable and could
            // shuffle control points that share a position.
            int index = 0;
            while (index < Count && colors[index].Position <= position)
            {
                index++;
            }
            colors.Insert(index, new GradientColor(position, color));
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
                // colors is sorted by position. index1 is the last color at or before position,
                // index2 is the first color at or after it.
                int index1 = 0, index2 = -1;
                for (int i = 0; i < Count; i++)
                {
                    if (colors[i].Position <= position)
                    {
                        index1 = i;
                    }
                    if (index2 == -1 && colors[i].Position >= position)
                    {
                        index2 = i;
                    }
                }
                if (index2 == -1)
                {
                    index2 = Count - 1;
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

        /// <summary>
        /// Sets the positions of all control points at once. positions[i] is paired with
        /// the color currently at index i, and the control points are sorted afterwards.
        /// </summary>
        public void SetPositions(double[] positions)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                if (i < Count)
                {
                    colors[i] = new GradientColor(positions[i], colors[i].Color);
                }
                else
                {
                    colors.Add(new GradientColor(positions[i], ColorBgra.Black));
                }
            }

            // insertion sort: stable, so control points sharing a position keep their order
            for (int i = 1; i < Count; i++)
            {
                GradientColor gc = colors[i];
                int j = i - 1;
                while (j >= 0 && colors[j].Position > gc.Position)
                {
                    colors[j + 1] = colors[j];
                    j--;
                }
                colors[j + 1] = gc;
            }
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