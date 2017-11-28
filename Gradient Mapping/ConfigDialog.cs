using PaintDotNet;
using PaintDotNet.Effects;
using pyrochild.effects.common;
using System;
using System.IO;
using System.Resources;
using System.Windows.Forms;
using System.Xml.Serialization;
using IShellService = PaintDotNet.AppModel.IShellService;

namespace pyrochild.effects.gradientmapping
{
    public partial class ConfigDialog : EffectConfigDialog
    {
        ResourceManager resourcemanager = Properties.Resources.ResourceManager;
        ConfigToken freshToken = new ConfigToken();

        public ConfigDialog()
        {
            InitializeComponent();
            this.Text = GradientMapping.StaticDialogName;
            
            foreach (string s in Enum.GetNames(typeof(Channel)))
            {
                modeComboBox.Items.Add(resourcemanager.GetString(s));
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            presetDropdown = new PresetDropdown<ConfigToken>(Services, Path.GetFileNameWithoutExtension(GetType().Assembly.CodeBase), freshToken, GetXao());
            // 
            // presetDropdown
            // 
            this.presetDropdown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.presetDropdown.Location = new System.Drawing.Point(5, 39);
            this.presetDropdown.Name = "presetDropdown";
            //this.presetDropdown.Size = new System.Drawing.Size(263, 21);
            this.presetDropdown.Size = this.modeComboBox.Size;
            //this.presetDropdown.DrawMode = DrawMode.OwnerDrawFixed;
            //this.presetDropdown.DrawItem += new DrawItemEventHandler(presetDropdown_DrawItem);

            this.Controls.Add(presetDropdown);
            this.Controls.SetChildIndex(presetDropdown, 0);

            SuspendTokenUpdates();
            AddDefaultPresets();
            presetDropdown.PresetChanged += presetDropdown_PresetChanged;
            presetDropdown.OnPresetChanged();
            ResumeTokenUpdates();
            base.OnLoad(e);
        }

        private void AddDefaultPresets()
        {
            Gradient rainbow = new Gradient();
            rainbow.Add(0, ColorBgra.Red);
            rainbow.Add(1 / 6.0, ColorBgra.Orange);
            rainbow.Add(1 / 3.0, ColorBgra.Yellow);
            rainbow.Add(.5, ColorBgra.Lime);
            rainbow.Add(2 / 3.0, ColorBgra.Blue);
            rainbow.Add(5 / 6.0, ColorBgra.Indigo);
            rainbow.Add(1,  ColorBgra.Violet);
            presetDropdown.AddPreset(new ConfigToken() { Gradient = rainbow }, "Rainbow");

            Gradient highcontrast = new Gradient();
            highcontrast.Add(0.6, ColorBgra.Black);
            highcontrast.Add(0.75, ColorBgra.White);
            presetDropdown.AddPreset(new ConfigToken() { Gradient = highcontrast }, "High Contrast");

            Gradient hot = new Gradient();
            hot.Add(0.2, ColorBgra.Black);
            hot.Add(0.75, ColorBgra.Red);
            hot.Add(0.95, ColorBgra.Yellow);
            hot.Add(1, ColorBgra.White);
            presetDropdown.AddPreset(new ConfigToken() { Gradient = hot }, "Hot");
        }

        private static XmlAttributeOverrides GetXao()
        {
            XmlAttributeOverrides xao = new XmlAttributeOverrides();

            //ignore Bgra as it's redundant
            XmlAttributes xa = new XmlAttributes();
            xa.XmlIgnore = true;
            xao.Add(typeof(ColorBgra), "Bgra", xa);

            //set these as attributes rather than elements
            xa = new XmlAttributes();
            xa.XmlAttribute = new XmlAttributeAttribute();
            xao.Add(typeof(ColorBgra), "B", xa);
            xao.Add(typeof(ColorBgra), "G", xa);
            xao.Add(typeof(ColorBgra), "R", xa);
            xao.Add(typeof(ColorBgra), "A", xa);

            return xao;
        }

        //void presetDropdown_DrawItem(object sender, DrawItemEventArgs e)
        //{
        //    Gradient gradient = presetDropdown[e.Index].Preset;
        //    if (gradient != null) gradient.DrawToGraphics(e.Graphics, e.Bounds);
        //}

        void presetDropdown_PresetChanged(object sender, PresetChangedEventArgs<ConfigToken> e)
        {
            InitDialogFromToken(e.Preset);
            FinishTokenUpdate();
        }

        protected override void InitDialogFromToken(EffectConfigToken effectToken)
        {
            SuspendTokenUpdates();
            ConfigToken token = (ConfigToken)effectToken;

            gradientControl.Gradient = token.Gradient;
            foreach (string s in Enum.GetNames(typeof(Channel)))
            {
                if (token.InputChannel.ToString() == s)
                {
                    modeComboBox.SelectedItem = resourcemanager.GetString(s);
                }
            }
            chkWrapOffset.Checked = token.Wrap;
            chkLockAlpha.Checked = token.LockAlpha;
            udOffset.Value = token.Offset;

            //first set the gradient
            if (presetDropdown == null)
            {
                OnLoad(EventArgs.Empty);
            }

            //set the preset name. if there's a preset, it will load it
            if (!token.Preset.IsNullOrEmpty())
            {
                presetDropdown.SetPresetByName(token.Preset);
            }

            ResumeTokenUpdates();
        }

        private int suspendTokenUpdatesCount = 0;
        private void ResumeTokenUpdates()
        {
            suspendTokenUpdatesCount--;
        }

        private void SuspendTokenUpdates()
        {
            suspendTokenUpdatesCount++;
        }
        private bool TokenUpdatesSuspended { get { return suspendTokenUpdatesCount > 0; } }

        protected override void InitialInitToken()
        {
            theEffectToken = new ConfigToken();
        }

        protected override void InitTokenFromDialog()
        {
            if (!TokenUpdatesSuspended)
            {
                ConfigToken token = (ConfigToken)theEffectToken;

                foreach (string s in Enum.GetNames(typeof(Channel)))
                {
                    if (modeComboBox.SelectedItem != null && modeComboBox.SelectedItem.ToString() == resourcemanager.GetString(s))
                    {
                        token.InputChannel = (Channel)Enum.Parse(typeof(Channel), s);
                    }
                }
                token.Offset = (int)udOffset.Value;
                token.Wrap = chkWrapOffset.Checked;
                token.LockAlpha = chkLockAlpha.Checked;
                token.Gradient = gradientControl.Gradient;
                if (token.Preset == presetDropdown.CurrentName)
                {
                    presetDropdown.Current = token;
                }
                token.Preset = presetDropdown.CurrentName;

            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void modeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FinishTokenUpdate();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Services.GetService<IShellService>().LaunchUrl(this, "http://forums.getpaint.net/index.php?showtopic=7291");
        }

        private void sldOffset_Scroll(object sender, EventArgs e)
        {
            udOffset.Value = sldOffset.Value;
        }

        private void udOffset_ValueChanged(object sender, EventArgs e)
        {
            sldOffset.Value = (int)udOffset.Value;
            FinishTokenUpdate();
        }

        private void btnResetOffset_Click(object sender, EventArgs e)
        {
            udOffset.Value = freshToken.Offset;
        }

        private void chkWrap_CheckedChanged(object sender, EventArgs e)
        {
            FinishTokenUpdate();
        }

        private void gradientControl_ValueChanged(object sender, EventArgs e)
        {
            presetDropdown.Current = (ConfigToken)theEffectToken;
            FinishTokenUpdate();
        }

        private void chkLockAlpha_CheckedChanged(object sender, EventArgs e)
        {
            FinishTokenUpdate();
        }
    }
}