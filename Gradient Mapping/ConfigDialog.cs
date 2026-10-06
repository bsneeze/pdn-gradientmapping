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
    public partial class ConfigDialog : EffectConfigForm<GradientMapping, ConfigToken>
    {
        ResourceManager resourcemanager = Properties.Resources.ResourceManager;
        ConfigToken freshToken = new ConfigToken();

        private float DpiScale => this.DeviceDpi / 96f;

        public ConfigDialog()
        {
            InitializeComponent();
            this.Load += (themeSender, themeArgs) => ThemeHelper.Apply(this);
            this.Shown += (themeSender, themeArgs) => ThemeHelper.Apply(this);
            this.Text = GradientMapping.StaticDialogName;

            foreach (string s in Enum.GetNames(typeof(Channel)))
            {
                modeComboBox.Items.Add(resourcemanager.GetString(s));
            }
        }

        private void EnsurePresetDropdown()
        {
            if (presetDropdown != null) return;

            presetDropdown = new PresetDropdown<ConfigToken>(Services, Path.GetFileNameWithoutExtension(GetType().Assembly.Location), freshToken, ConfigToken.GetXao());
            //
            // presetDropdown
            //
            this.presetDropdown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            // presetDropdown is created in code and misses the designer's DPI scaling, so position it
            // from gradientControl's scaled bounds.
            this.presetDropdown.Location = new System.Drawing.Point(gradientControl.Left + 4, gradientControl.Bottom + 1);
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
            presetDropdown.AddDefaultPreset(new ConfigToken() { Gradient = rainbow }, "Rainbow");

            Gradient highcontrast = new Gradient();
            highcontrast.Add(0.6, ColorBgra.Black);
            highcontrast.Add(0.75, ColorBgra.White);
            presetDropdown.AddDefaultPreset(new ConfigToken() { Gradient = highcontrast }, "High Contrast");

            Gradient hot = new Gradient();
            hot.Add(0.2, ColorBgra.Black);
            hot.Add(0.75, ColorBgra.Red);
            hot.Add(0.95, ColorBgra.Yellow);
            hot.Add(1, ColorBgra.White);
            presetDropdown.AddDefaultPreset(new ConfigToken() { Gradient = hot }, "Hot");

            Gradient synthwave = new Gradient();
            synthwave.Add(0, ColorBgra.Indigo);
            synthwave.Add(1 / 3.0, ColorBgra.DeepPink);
            synthwave.Add(2 / 3.0, ColorBgra.OrangeRed);
            synthwave.Add(1, ColorBgra.Cyan);
            presetDropdown.AddDefaultPreset(new ConfigToken() { Gradient = synthwave }, "Synthwave");

            Gradient sepia = new Gradient();
            sepia.Add(0, ColorBgra.FromBgr(9, 17, 29));
            sepia.Add(0.5, ColorBgra.FromBgr(20, 66, 112));
            sepia.Add(1, ColorBgra.FromBgr(151, 196, 222));
            presetDropdown.AddDefaultPreset(new ConfigToken() { Gradient = sepia }, "Sepia");

            Gradient duotoneBlue = new Gradient();
            duotoneBlue.Add(0, ColorBgra.Navy);
            duotoneBlue.Add(1, ColorBgra.White);
            presetDropdown.AddDefaultPreset(new ConfigToken() { Gradient = duotoneBlue }, "Duotone Blue");

            Gradient thermal = new Gradient();
            thermal.Add(0, ColorBgra.Black);
            thermal.Add(0.2, ColorBgra.Blue);
            thermal.Add(0.4, ColorBgra.Lime);
            thermal.Add(0.6, ColorBgra.Yellow);
            thermal.Add(0.8, ColorBgra.Red);
            thermal.Add(1, ColorBgra.White);
            presetDropdown.AddDefaultPreset(new ConfigToken() { Gradient = thermal }, "Thermal");

            Gradient cyanotype = new Gradient();
            cyanotype.Add(0, ColorBgra.FromBgr(60, 20, 10));
            cyanotype.Add(1, ColorBgra.FromBgr(250, 235, 220));
            presetDropdown.AddDefaultPreset(new ConfigToken() { Gradient = cyanotype }, "Cyanotype");

            Gradient posterizeGrays = new Gradient();
            posterizeGrays.Add(0, ColorBgra.Black);
            posterizeGrays.Add(1 / 3.0, ColorBgra.FromBgr(85, 85, 85));
            posterizeGrays.Add(2 / 3.0, ColorBgra.FromBgr(170, 170, 170));
            posterizeGrays.Add(1, ColorBgra.White);
            presetDropdown.AddDefaultPreset(new ConfigToken() { Gradient = posterizeGrays }, "Posterize Grays");

            Gradient cottonCandy = new Gradient();
            cottonCandy.Add(0, ColorBgra.FromBgr(213, 182, 255));
            cottonCandy.Add(0.5, ColorBgra.FromBgr(230, 170, 200));
            cottonCandy.Add(1, ColorBgra.FromBgr(255, 220, 150));
            presetDropdown.AddDefaultPreset(new ConfigToken() { Gradient = cottonCandy }, "Cotton Candy");

            Gradient toxic = new Gradient();
            toxic.Add(0, ColorBgra.Black);
            toxic.Add(0.5, ColorBgra.FromBgr(20, 255, 100));
            toxic.Add(1, ColorBgra.GreenYellow);
            presetDropdown.AddDefaultPreset(new ConfigToken() { Gradient = toxic }, "Toxic");

            Gradient fire = new Gradient();
            fire.Add(0, ColorBgra.Black);
            fire.Add(0.4, ColorBgra.Red);
            fire.Add(0.7, ColorBgra.OrangeRed);
            fire.Add(1, ColorBgra.Yellow);
            presetDropdown.AddDefaultPreset(new ConfigToken() { Gradient = fire }, "Fire");

            Gradient vaporwave = new Gradient();
            vaporwave.Add(0, ColorBgra.Cyan);
            vaporwave.Add(0.5, ColorBgra.Magenta);
            vaporwave.Add(1, ColorBgra.Indigo);
            presetDropdown.AddDefaultPreset(new ConfigToken() { Gradient = vaporwave }, "Vaporwave");
        }

        //void presetDropdown_DrawItem(object sender, DrawItemEventArgs e)
        //{
        //    Gradient gradient = presetDropdown[e.Index].Preset;
        //    if (gradient != null) gradient.DrawToGraphics(e.Graphics, e.Bounds);
        //}

        void presetDropdown_PresetChanged(object sender, PresetChangedEventArgs<ConfigToken> e)
        {
            OnUpdateDialogFromToken(e.Preset);
            UpdateTokenFromDialog();
        }

        protected override void OnUpdateDialogFromToken(ConfigToken token)
        {
            SuspendTokenUpdates();

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
            EnsurePresetDropdown();

            //set the preset name. if there's a preset, it will load it
            if (!string.IsNullOrEmpty(token.Preset))
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

        protected override EffectConfigToken OnCreateInitialToken()
        {
            return new ConfigToken();
        }

        protected override void OnUpdateTokenFromDialog(ConfigToken token)
        {
            if (!TokenUpdatesSuspended)
            {
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
            UpdateTokenFromDialog();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ((IShellService)Services.GetService(typeof(IShellService))).LaunchUrl(this, "http://forums.getpaint.net/index.php?showtopic=7291");
        }

        private void sldOffset_Scroll(object sender, EventArgs e)
        {
            udOffset.Value = sldOffset.Value;
        }

        private void udOffset_ValueChanged(object sender, EventArgs e)
        {
            sldOffset.Value = (int)udOffset.Value;
            UpdateTokenFromDialog();
        }

        private void btnResetOffset_Click(object sender, EventArgs e)
        {
            udOffset.Value = freshToken.Offset;
        }

        private void chkWrap_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTokenFromDialog();
        }

        private void gradientControl_ValueChanged(object sender, EventArgs e)
        {
            ConfigToken current = new ConfigToken();
            OnUpdateTokenFromDialog(current);
            presetDropdown.Current = current;
            UpdateTokenFromDialog();
        }

        private void chkLockAlpha_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTokenFromDialog();
        }
    }
}