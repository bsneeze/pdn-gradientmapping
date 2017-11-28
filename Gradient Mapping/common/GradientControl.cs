using PaintDotNet;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace pyrochild.effects.common
{
    public partial class GradientControl : Control
    {
        private const int nubSize = 10;

        private Gradient gradient;
        private int selectedIndex = -1;
        private int savedIndex = -1;
        private bool tracking;
        private Point lastMouse;
        private Point savedMouse;
        private bool cmdkeydown;

        public GradientControl()
        {
            this.SetStyle(
                ControlStyles.Selectable
                | ControlStyles.UserPaint
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.UserMouse, true);

            InitializeComponent();

            gradient = new Gradient();
        }

        [DefaultValue(typeof(Color), "ControlDarkDark")]
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Gradient Gradient
        {
            get { return gradient; }
            set
            {
                gradient = value;
                Invalidate();
            }
        }

        private int suspendEvents;
        private bool EventsSuspended
        {
            get
            {
                return suspendEvents > 0;
            }
        }
        private void SuspendEvents()
        {
            suspendEvents++;
        }
        private void ResumeEvents()
        {
            suspendEvents--;
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            if (selectedIndex == -1) selectedIndex = 0;
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            selectedIndex = -1;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (selectedIndex != -1
                && e.Button == MouseButtons.Left)
            {
                tracking = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                tracking = false;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            selectedIndex = -1;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Location != lastMouse)
            {
                if (gradient != null && gradient.Count > 0)
                {
                    int gWidth = ClientRectangle.Width - 1 - 2 * nubSize;

                    if (tracking)
                    {
                        selectedIndex = gradient.SetPosition(selectedIndex, ((e.X - nubSize) / (float)gWidth).Clamp(0, 1));
                        OnValueChanged();
                    }
                    else
                    {
                        selectedIndex = -1;
                        for (int i = 0; i < gradient.Count; i++)
                        {
                            if (e.X > gradient.GetPosition(i) * gWidth + nubSize / 2
                                && e.X < gradient.GetPosition(i) * gWidth + 3 * nubSize / 2)
                            {
                                selectedIndex = i;
                            }
                        }
                    }
                }
                else
                {
                    selectedIndex = -1;
                }

                lastMouse = e.Location;
                Invalidate();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (selectedIndex >= 0)
                {
                    cmNub.Show(this, e.Location);
                }
                else
                {
                    cmControl.Show(this, e.Location);
                }
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                tracking = false;
                lastMouse = new Point(-1, -1);
                OnMouseMove(e);
                if (selectedIndex >= 0)
                {
                    ChangeColor(selectedIndex);
                }
                else
                {
                    AddColor(e.Location);
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool handled = false;

            int gWidth = ClientRectangle.Width - 1 - 2 * nubSize;
            float pixeldelta = 1 / (float)gWidth;
            int alphadelta = 4;
            if ((keyData & Keys.Control) != Keys.None)
            {
                pixeldelta *= 10;
                alphadelta *= 4;
            }

            Keys keyCode = Keys.KeyCode & keyData;

            switch (keyCode)
            {
                case Keys.Left:

                    if (selectedIndex >= 0 && selectedIndex < gradient.Count)
                    {
                        selectedIndex = gradient.SetPosition(selectedIndex, (gradient.GetPosition(selectedIndex) - pixeldelta).Clamp(0, 1));
                        handled = true;
                        cmdkeydown = true;
                        Invalidate();
                    }
                    break;

                case Keys.Right:
                    if (selectedIndex >= 0 && selectedIndex < gradient.Count)
                    {
                        selectedIndex = gradient.SetPosition(selectedIndex, (gradient.GetPosition(selectedIndex) + pixeldelta).Clamp(0, 1));
                        handled = true;
                        cmdkeydown = true;
                        Invalidate();
                    }
                    break;
                case Keys.Apps:
                    Point pt = GetNubLocation(selectedIndex);
                    OnMouseClick(new MouseEventArgs(MouseButtons.Right, 0, pt.X, pt.Y, 0));
                    handled = true;
                    break;

                case Keys.Tab:
                    handled = ProcessTabKey((keyData & Keys.Modifiers) == Keys.None);
                    break;

                case Keys.OemMinus:
                    handled = ProcessTabKey(false);
                    break;

                case Keys.Oemplus:
                    handled = ProcessTabKey(true);
                    break;

                case Keys.Up:
                    if (selectedIndex >= 0 && selectedIndex < gradient.Count)
                    {
                        ColorBgra currentColor = gradient.GetColor(selectedIndex);
                        gradient.SetColor(selectedIndex, currentColor.NewAlpha((currentColor.A + alphadelta).ClampToByte()));
                        cmdkeydown = true;
                        handled = true;
                        Invalidate();
                    }
                    break;

                case Keys.Down:
                    if (selectedIndex >= 0 && selectedIndex < gradient.Count)
                    {
                        ColorBgra currentColor = gradient.GetColor(selectedIndex);
                        gradient.SetColor(selectedIndex, currentColor.NewAlpha((currentColor.A - alphadelta).ClampToByte()));
                        handled = true;
                        cmdkeydown = true;
                        Invalidate();
                    }
                    break;

                case Keys.Enter:

                    if (selectedIndex >= 0 && selectedIndex < gradient.Count)
                    {
                        pt = GetNubLocation(selectedIndex);
                        OnMouseDoubleClick(new MouseEventArgs(MouseButtons.Left, 0, pt.X, pt.Y, 0));
                        handled = true;
                    }
                    break;
            }

            if ((int)keyData <= 57 && (int)keyData >= 48)
            {
                ProcessNumberKeys(keyData);
                handled = true;
            }

            return handled || base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (cmdkeydown && e.Modifiers == Keys.None)
            {
                cmdkeydown = false;
                OnValueChanged();
            }
        }

        protected bool ProcessTabKey(bool forward)
        {
            bool retval = false;

            //if (selectedIndex == -1) selectedIndex = savedIndex;
            if (forward && selectedIndex < gradient.Count - 1)
            {
                selectedIndex++;

                Invalidate();
                retval = true;
            }
            else if (!forward && selectedIndex > 0)
            {
                selectedIndex--;

                Invalidate();
                retval = true;
            }
            //savedIndex = selectedIndex;
            return retval;
        }

        protected void ProcessNumberKeys(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.D0:
                    selectedIndex = 9;
                    break;
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                    selectedIndex = (int)keyCode - 49;
                    break;
            }
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Pen outlinepen = new Pen(this.ForeColor);
            Pen selectednuboutlinepen = new Pen(Color.White);
            SolidBrush nubbrush = new SolidBrush(Color.White);
            SolidBrush selectednubbrush = new SolidBrush(Color.White);

            Graphics g = e.Graphics;
            g.Clear(this.BackColor);

            Rectangle rOutline = new Rectangle(
                nubSize,
                nubSize,
                ClientRectangle.Width - 1 - 2 * nubSize,
                ClientRectangle.Height - 1 - 2 * nubSize);
            g.DrawRectangle(outlinepen, rOutline);

            if (this.Focused)
            {
                using (Pen focusedoutlinepen = new Pen(SystemColors.Highlight))
                {
                    focusedoutlinepen.DashStyle = DashStyle.Dash;
                    g.DrawRectangle(focusedoutlinepen, Rectangle.Inflate(rOutline, 2, 2));
                }
            }

            if (gradient != null && gradient.Count > 0)
            {
                Rectangle rGradient = Rectangle.Inflate(rOutline, -1, -1);
                gradient.DrawToGraphics(g, rGradient);

                g.SmoothingMode = SmoothingMode.AntiAlias;

                for (int i = 0; i < gradient.Count; i++)
                {
                    double position = gradient.GetPosition(i);
                    selectednuboutlinepen.Color = (gradient.GetColor(i)).ToColor().ToOpaqueColor();
                    nubbrush.Color = selectednuboutlinepen.Color;

                    // if nubs are very close or on top of one another, make one slightly bigger 
                    // so they can be seen
                    float ournubSize = nubSize;
                    if (i < gradient.Count - 1 && (gradient.GetPosition(i + 1) - position) <= nubSize / (float)rGradient.Width)
                    {
                        float diff = 10 - (float)(gradient.GetPosition(i + 1) - position) * rGradient.Width;
                        ournubSize += (float)(0.5 * nubSize * Math.Sin(diff * Math.PI / 20));
                    }
                    float offsetX = 0.5f * ournubSize;
                    float offsetY = (float)Math.Sqrt(0.75) * ournubSize;

                    //it's a triangle. for the top nub.
                    PointF[] markerPolygonTop = new PointF[] {
                                    new PointF((float)(position * (rGradient.Width + 1) + nubSize),
                                        (rGradient.Top+3)),
                                    new PointF((float)(position * (rGradient.Width + 1) - offsetX + nubSize),
                                        (rGradient.Top - offsetY+3)),
                                    new PointF((float)(position * (rGradient.Width + 1) + offsetX + nubSize),
                                        (rGradient.Top - offsetY+3)) };

                    //it's another triangle. for the bottom.
                    PointF[] markerPolygonBottom = new PointF[] {
                                    new PointF((float)(position * (rGradient.Width + 1) + nubSize),
                                        (rGradient.Bottom - 3)),
                                    new PointF((float)(position * (rGradient.Width + 1) + nubSize - offsetX),
                                        (rGradient.Bottom - 3 + offsetY)),
                                    new PointF((float)(position * (rGradient.Width + 1) + nubSize + offsetX),
                                        (rGradient.Bottom - 3 + offsetY)) };

                    if (selectedIndex == i)
                    {
                        //draw this way for selected nub...
                        g.FillPolygon(selectednubbrush, markerPolygonTop);
                        g.DrawPolygon(selectednuboutlinepen, markerPolygonTop);
                        g.FillPolygon(selectednubbrush, markerPolygonBottom);
                        g.DrawPolygon(selectednuboutlinepen, markerPolygonBottom);
                    }
                    else
                    {
                        //and this way for others!
                        g.FillPolygon(nubbrush, markerPolygonTop);
                        g.DrawPolygon(outlinepen, markerPolygonTop);
                        g.FillPolygon(nubbrush, markerPolygonBottom);
                        g.DrawPolygon(outlinepen, markerPolygonBottom);
                    }
                }
                outlinepen.Dispose();
                selectednubbrush.Dispose();
                nubbrush.Dispose();
                selectednuboutlinepen.Dispose();
            }
        }

        private void AddColor(Point position)
        {
            ColorDialog cd = new ColorDialog(true);
            cd.Color = ColorBgra.Black;
            if (cd.ShowDialog(this) == DialogResult.OK)
            {
                int gWidth = ClientRectangle.Width - 1 - 2 * nubSize;
                gradient.Add(((position.X - nubSize / 2f) / gWidth).Clamp(0, 1), cd.Color);
                Invalidate();
                OnValueChanged();
            }
        }

        private void ChangeColor(int index)
        {
            ColorDialog cd = new ColorDialog(true);
            cd.Color = gradient.GetColor(index);
            if (cd.ShowDialog(this) == DialogResult.OK)
            {
                gradient.SetColor(index, cd.Color);
                Invalidate();
                OnValueChanged();
            }
        }

        private Point GetNubLocation(int index)
        {
            if (index == -1)
            {
                return lastMouse;
            }
            else
            {
                int gWidth = ClientRectangle.Width - 1 - 2 * nubSize;

                return new Point((int)(gradient.GetPosition(index) * gWidth) + nubSize, this.Height);
            }
        }

        void changeColor_Click(object sender, EventArgs e)
        {
            ChangeColor(savedIndex);
            Invalidate();
            OnValueChanged();
        }

        void deleteColor_Click(object sender, EventArgs e)
        {
            gradient.RemoveAt(savedIndex);
            Invalidate();
            OnValueChanged();
        }

        void spreadColors_Click(object sender, EventArgs e)
        {
            gradient.Spread();
            Invalidate();
            OnValueChanged();
        }

        void clearColors_Click(object sender, EventArgs e)
        {
            gradient.SetDefault();
            Invalidate();
            OnValueChanged();
        }

        void reverseColors_Click(object sender, EventArgs e)
        {
            gradient.Reverse();
            Invalidate();
            OnValueChanged();
        }

        void addColor_Click(object sender, EventArgs e)
        {
            AddColor(savedMouse);
        }

        void cm_Opening(object sender, CancelEventArgs e)
        {
            savedIndex = selectedIndex;
            savedMouse = lastMouse;
        }

        public event EventHandler ValueChanged;
        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, EventArgs.Empty);
            }
        }
    }
}