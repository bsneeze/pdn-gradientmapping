using pyrochild.effects.common;
namespace pyrochild.effects.gradientmapping
{
    partial class ConfigDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.modeComboBox = new System.Windows.Forms.ComboBox();
            this.hdr0 = new System.Windows.Forms.Label();
            this.hdr1 = new System.Windows.Forms.Label();
            this.btnResetOffset = new System.Windows.Forms.Button();
            this.udOffset = new System.Windows.Forms.NumericUpDown();
            this.sldOffset = new System.Windows.Forms.TrackBar();
            this.chkWrapOffset = new System.Windows.Forms.CheckBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.chkLockAlpha = new System.Windows.Forms.CheckBox();
            this.gradientControl = new GradientControl();
            ((System.ComponentModel.ISupportInitialize)(this.udOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sldOffset)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCancel.Location = new System.Drawing.Point(267, 215);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(81, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnOk.Location = new System.Drawing.Point(180, 215);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(81, 23);
            this.btnOk.TabIndex = 12;
            this.btnOk.Text = "OK";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // modeComboBox
            // 
            this.modeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.modeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.modeComboBox.Location = new System.Drawing.Point(5, 90);
            this.modeComboBox.Name = "modeComboBox";
            this.modeComboBox.Size = new System.Drawing.Size(344, 21);
            this.modeComboBox.TabIndex = 10;
            this.modeComboBox.SelectedIndexChanged += new System.EventHandler(this.modeComboBox_SelectedIndexChanged);
            // 
            // hdr0
            // 
            this.hdr0.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.hdr0.Location = new System.Drawing.Point(4, 70);
            this.hdr0.Name = "hdr0";
            this.hdr0.Size = new System.Drawing.Size(352, 14);
            this.hdr0.TabIndex = 9;
            this.hdr0.TabStop = false;
            this.hdr0.Text = "Source";
            // 
            // hdr1
            // 
            this.hdr1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.hdr1.Location = new System.Drawing.Point(4, 123);
            this.hdr1.Name = "hdr1";
            this.hdr1.Size = new System.Drawing.Size(352, 14);
            this.hdr1.TabIndex = 5;
            this.hdr1.TabStop = false;
            this.hdr1.Text = "Offset";
            // 
            // btnResetOffset
            // 
            this.btnResetOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetOffset.Location = new System.Drawing.Point(267, 165);
            this.btnResetOffset.Name = "btnResetOffset";
            this.btnResetOffset.Size = new System.Drawing.Size(81, 20);
            this.btnResetOffset.TabIndex = 6;
            this.btnResetOffset.Text = "Reset";
            this.btnResetOffset.Click += new System.EventHandler(this.btnResetOffset_Click);
            // 
            // udOffset
            // 
            this.udOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.udOffset.Location = new System.Drawing.Point(267, 140);
            this.udOffset.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.udOffset.Minimum = new decimal(new int[] {
            255,
            0,
            0,
            -2147483648});
            this.udOffset.Name = "udOffset";
            this.udOffset.Size = new System.Drawing.Size(81, 20);
            this.udOffset.TabIndex = 7;
            this.udOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.udOffset.Value = new decimal(new int[] {
            255,
            0,
            0,
            -2147483648});
            this.udOffset.ValueChanged += new System.EventHandler(this.udOffset_ValueChanged);
            // 
            // sldOffset
            // 
            this.sldOffset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sldOffset.LargeChange = 20;
            this.sldOffset.Location = new System.Drawing.Point(-1, 140);
            this.sldOffset.Maximum = 255;
            this.sldOffset.Minimum = -255;
            this.sldOffset.Name = "sldOffset";
            this.sldOffset.Size = new System.Drawing.Size(256, 45);
            this.sldOffset.TabIndex = 8;
            this.sldOffset.TickFrequency = 32;
            this.sldOffset.Value = -255;
            this.sldOffset.Scroll += new System.EventHandler(this.sldOffset_Scroll);
            // 
            // chkWrapOffset
            // 
            this.chkWrapOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkWrapOffset.AutoSize = true;
            this.chkWrapOffset.Location = new System.Drawing.Point(12, 173);
            this.chkWrapOffset.Name = "chkWrapOffset";
            this.chkWrapOffset.Size = new System.Drawing.Size(52, 17);
            this.chkWrapOffset.TabIndex = 4;
            this.chkWrapOffset.Text = "Wrap";
            this.chkWrapOffset.UseVisualStyleBackColor = true;
            this.chkWrapOffset.CheckedChanged += new System.EventHandler(this.chkWrap_CheckedChanged);
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(1, 225);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(45, 13);
            this.linkLabel1.TabIndex = 3;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Donate!";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // chkLockAlpha
            // 
            this.chkLockAlpha.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkLockAlpha.AutoSize = true;
            this.chkLockAlpha.Location = new System.Drawing.Point(12, 193);
            this.chkLockAlpha.Name = "chkLockAlpha";
            this.chkLockAlpha.Size = new System.Drawing.Size(98, 17);
            this.chkLockAlpha.TabIndex = 1;
            this.chkLockAlpha.Text = "Preserve Alpha";
            this.chkLockAlpha.UseVisualStyleBackColor = true;
            this.chkLockAlpha.CheckedChanged += new System.EventHandler(this.chkLockAlpha_CheckedChanged);
            // 
            // gradientControl
            // 
            this.gradientControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gradientControl.Cursor = System.Windows.Forms.Cursors.Default;
            this.gradientControl.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.gradientControl.Location = new System.Drawing.Point(1, 0);
            this.gradientControl.Name = "gradientControl";
            this.gradientControl.Size = new System.Drawing.Size(354, 38);
            this.gradientControl.TabIndex = 13;
            this.gradientControl.ValueChanged += new System.EventHandler(this.gradientControl_ValueChanged);
            // 
            // ConfigDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(356, 243);
            this.Controls.Add(this.chkLockAlpha);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.chkWrapOffset);
            this.Controls.Add(this.hdr1);
            this.Controls.Add(this.btnResetOffset);
            this.Controls.Add(this.udOffset);
            this.Controls.Add(this.sldOffset);
            this.Controls.Add(this.hdr0);
            this.Controls.Add(this.modeComboBox);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.gradientControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximumSize = new System.Drawing.Size(32767, 281);
            this.MinimumSize = new System.Drawing.Size(291, 281);
            this.Name = "ConfigDialog";
            this.Controls.SetChildIndex(this.gradientControl, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.modeComboBox, 0);
            this.Controls.SetChildIndex(this.hdr0, 0);
            this.Controls.SetChildIndex(this.sldOffset, 0);
            this.Controls.SetChildIndex(this.udOffset, 0);
            this.Controls.SetChildIndex(this.btnResetOffset, 0);
            this.Controls.SetChildIndex(this.hdr1, 0);
            this.Controls.SetChildIndex(this.chkWrapOffset, 0);
            this.Controls.SetChildIndex(this.linkLabel1, 0);
            this.Controls.SetChildIndex(this.chkLockAlpha, 0);
            ((System.ComponentModel.ISupportInitialize)(this.udOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sldOffset)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.Button btnCancel;
        protected System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.ComboBox modeComboBox;
        private System.Windows.Forms.Label hdr0;
        private System.Windows.Forms.Label hdr1;
        private System.Windows.Forms.Button btnResetOffset;
        private System.Windows.Forms.NumericUpDown udOffset;
        private System.Windows.Forms.TrackBar sldOffset;
        private System.Windows.Forms.CheckBox chkWrapOffset;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private pyrochild.effects.common.GradientControl gradientControl;
        private pyrochild.effects.common.PresetDropdown<pyrochild.effects.common.Gradient> presetDropdown;
        private System.Windows.Forms.CheckBox chkLockAlpha;

    }
}