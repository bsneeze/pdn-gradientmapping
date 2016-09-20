namespace pyrochild.effects.common
{
    partial class GradientControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cmControl = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addColorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.separator1 = new System.Windows.Forms.ToolStripSeparator();
            this.spreadColorsMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.separator3 = new System.Windows.Forms.ToolStripSeparator();
            this.clearColorsMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.cmNub = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.changeColorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteColorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.separator2 = new System.Windows.Forms.ToolStripSeparator();
            this.spreadColorsMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.separator4 = new System.Windows.Forms.ToolStripSeparator();
            this.clearColorsMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.reverseColorsMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.reverseColorsMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.cmControl.SuspendLayout();
            this.cmNub.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmControl
            // 
            this.cmControl.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addColorMenuItem,
            this.separator1,
            this.spreadColorsMenuItem1,
            this.reverseColorsMenuItem1,
            this.separator3,
            this.clearColorsMenuItem1});
            this.cmControl.Name = "cmControl";
            this.cmControl.Opening += new System.ComponentModel.CancelEventHandler(cm_Opening);
            this.cmControl.Size = new System.Drawing.Size(153, 126);
            // 
            // addColorToolStripMenuItem
            // 
            this.addColorMenuItem.Name = "addColorToolStripMenuItem";
            this.addColorMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addColorMenuItem.Text = "Add Color";
            this.addColorMenuItem.Click += new System.EventHandler(addColor_Click);
            // 
            // toolStripMenuItem1
            // 
            this.separator1.Name = "toolStripMenuItem1";
            this.separator1.Size = new System.Drawing.Size(149, 6);
            // 
            // spreadToolStripMenuItem
            // 
            this.spreadColorsMenuItem1.Name = "spreadToolStripMenuItem";
            this.spreadColorsMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.spreadColorsMenuItem1.Text = "Spread";
            this.spreadColorsMenuItem1.Click += new System.EventHandler(spreadColors_Click);
            // 
            // toolStripMenuItem3
            // 
            this.separator3.Name = "toolStripMenuItem3";
            this.separator3.Size = new System.Drawing.Size(149, 6);
            // 
            // clearToolStripMenuItem
            // 
            this.clearColorsMenuItem1.Name = "clearToolStripMenuItem";
            this.clearColorsMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.clearColorsMenuItem1.Text = "Clear";
            this.clearColorsMenuItem1.Click += new System.EventHandler(clearColors_Click);
            // 
            // cmNub
            // 
            this.cmNub.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeColorMenuItem,
            this.deleteColorMenuItem,
            this.separator2,
            this.spreadColorsMenuItem2,
            this.reverseColorsMenuItem2,
            this.separator4,
            this.clearColorsMenuItem2});
            this.cmNub.Name = "cmNub";
            this.cmNub.Opening += new System.ComponentModel.CancelEventHandler(cm_Opening);
            this.cmNub.Size = new System.Drawing.Size(148, 104);
            // 
            // changeColorToolStripMenuItem
            // 
            this.changeColorMenuItem.Name = "changeColorToolStripMenuItem";
            this.changeColorMenuItem.Size = new System.Drawing.Size(147, 22);
            this.changeColorMenuItem.Text = "Change Color";
            this.changeColorMenuItem.Click += new System.EventHandler(changeColor_Click);
            // 
            // deleteToolStripMenuItem1
            // 
            this.deleteColorMenuItem.Name = "deleteToolStripMenuItem1";
            this.deleteColorMenuItem.Size = new System.Drawing.Size(147, 22);
            this.deleteColorMenuItem.Text = "Remove";
            this.deleteColorMenuItem.Click += new System.EventHandler(deleteColor_Click);
            // 
            // toolStripMenuItem2
            // 
            this.separator2.Name = "toolStripMenuItem2";
            this.separator2.Size = new System.Drawing.Size(144, 6);
            // 
            // spreadColorsToolStripMenuItem
            // 
            this.spreadColorsMenuItem2.Name = "spreadColorsToolStripMenuItem";
            this.spreadColorsMenuItem2.Size = new System.Drawing.Size(147, 22);
            this.spreadColorsMenuItem2.Text = "Spread";
            this.spreadColorsMenuItem2.Click += new System.EventHandler(spreadColors_Click);
            // 
            // toolStripMenuItem4
            // 
            this.separator4.Name = "toolStripMenuItem4";
            this.separator4.Size = new System.Drawing.Size(144, 6);
            // 
            // clearToolStripMenuItem1
            // 
            this.clearColorsMenuItem2.Name = "clearToolStripMenuItem1";
            this.clearColorsMenuItem2.Size = new System.Drawing.Size(147, 22);
            this.clearColorsMenuItem2.Text = "Clear";
            this.clearColorsMenuItem2.Click += new System.EventHandler(clearColors_Click);
            // 
            // reverseToolStripMenuItem
            // 
            this.reverseColorsMenuItem1.Name = "reverseToolStripMenuItem";
            this.reverseColorsMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.reverseColorsMenuItem1.Text = "Reverse";
            this.reverseColorsMenuItem1.Click += new System.EventHandler(reverseColors_Click);
            // 
            // reverseColorsToolStripMenuItem
            // 
            this.reverseColorsMenuItem2.Name = "reverseToolStripMenuItem";
            this.reverseColorsMenuItem2.Size = new System.Drawing.Size(152, 22);
            this.reverseColorsMenuItem2.Text = "Reverse";
            this.reverseColorsMenuItem2.Click += new System.EventHandler(reverseColors_Click);
            // 
            // GradientControl
            // 
            this.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Name = "GradientControl";
            this.cmControl.ResumeLayout(false);
            this.cmNub.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip cmControl;
        private System.Windows.Forms.ToolStripMenuItem addColorMenuItem;
        private System.Windows.Forms.ContextMenuStrip cmNub;
        private System.Windows.Forms.ToolStripMenuItem changeColorMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteColorMenuItem;
        private System.Windows.Forms.ToolStripSeparator separator1;
        private System.Windows.Forms.ToolStripMenuItem spreadColorsMenuItem1;
        private System.Windows.Forms.ToolStripSeparator separator2;
        private System.Windows.Forms.ToolStripMenuItem spreadColorsMenuItem2;
        private System.Windows.Forms.ToolStripSeparator separator3;
        private System.Windows.Forms.ToolStripMenuItem clearColorsMenuItem1;
        private System.Windows.Forms.ToolStripSeparator separator4;
        private System.Windows.Forms.ToolStripMenuItem clearColorsMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem reverseColorsMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem reverseColorsMenuItem2;
    }
}