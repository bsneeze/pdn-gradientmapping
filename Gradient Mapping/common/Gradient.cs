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
        public Gradient()
        {
            Colors = new List<ColorBgra>();
            Positions = new List<double>();
        }

        public List<ColorBgra> Colors;        
        public List<double> Positions;

        public int Count
        {
            get
            {
                return Math.Min(Colors.Count, Positions.Count);
            }
        }

        /// <summary>
        /// Sorts the colors into numerical order
        /// Also ensures there is no "stacking"
        /// </summary>
        /// <param name="index">any given nub's index (usually the selected one)</param>
        /// <returns>if "index" is changed via this routine, the return will be the new value
        /// (for tracking, this ensures that nubs don't get switched if the order does)</returns>
        public int Sort(int index)
        {
            int retval = index;
            bool b;
            do
            {
                b = false;
                for (int i = 0; i < this.Count - 1; i++)
                {
                    if (Positions[i] > Positions[i + 1])
                    {
                        double tmp1 = Positions[i];
                        Positions[i] = Positions[i + 1];
                        Positions[i + 1] = tmp1;

                        ColorBgra tmp2 = Colors[i];
                        Colors[i] = Colors[i + 1];
                        Colors[i + 1] = tmp2;

                        b = true;
                        if (retval == i)
                        {
                            retval++;
                        }
                        else if (retval == i + 1)
                        {
                            retval--;
                        }
                    }
                    else if (Positions[i] == Positions[i + 1])
                    {
                        if (Positions[i] != 1)
                        {
                            Positions[i + 1] += 1e-16 - 1e-32;
                        }
                        else
                        {
                            Positions[i + 1] -= 1e-16 - 1e-32;
                        }
                    }
                }
            } while (b);
            return retval;
        }

        public void Sort()
        {
            Sort(0);
        }

        public void Reverse()
        {
            Positions = Positions.ConvertAll<double>(d => { return 1 - d; });
            Sort(0);
        }

        public void Add(double position, ColorBgra color)
        {
            Colors.Add(color);
            Positions.Add(position);
            Sort();
        }

        public void RemoveAt(int index)
        {
            Colors.RemoveAt(index);
            Positions.RemoveAt(index);
        }

        /// <summary>
        /// returns the blended color from the gradient
        /// --Make sure Sort() has been called!--
        /// </summary>
        /// <param name="position">position to get color from (0.0-1.0)</param>
        /// <returns>the color...</returns>
        public ColorBgra GetColor(double position)
        {
            if (this.Count > 0)
            {
                int index1 = 0, index2 = Positions.Count - 1;
                for (int i = 0; i < this.Count; i++)
                {
                    if ((Positions[i] <= position) && (Positions[i] >= Positions[index1]))
                    {
                        index1 = i;
                    }
                    if ((Positions[i] >= position) && (Positions[i] <= Positions[index2]))
                    {
                        index2 = i;
                    }
                }
                if (index1 == index2)
                {
                    return Colors[index1];
                }

                return ColorBgraBlender.Blend(this.Colors[index1], this.Colors[index2], (double)((position - this.Positions[index1]) / (this.Positions[index2] - this.Positions[index1])));
            }
            else
            {
                throw new InvalidOperationException("You must first add colors to the gradient.");
            }
        }

        public object Clone()
        {
            Gradient retval = new Gradient();
            retval.Colors.AddRange(Colors);
            retval.Positions.AddRange(Positions);
            return retval;
        }

        /// <summary>
        /// Evenly spaces all of the colors.
        /// </summary>
        public void Spread()
        {
            double space = 1.0 / (double)(this.Count - 1);

            for (int i = 0; i < this.Count; i++)
            {
                Positions[i] = i * space;
            }
        }

        public void Clear()
        {
            Colors.Clear();
            Positions.Clear();
        }

        public void SetDefault()
        {
            Clear();
            Colors.AddRange(new ColorBgra[] { ColorBgra.Black, ColorBgra.White });
            Positions.AddRange(new double[] { 0, 1 });
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