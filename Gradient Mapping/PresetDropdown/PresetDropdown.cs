using PaintDotNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace pyrochild.effects.common
{
    public sealed class PresetDropdown<T> : Control
        where T : ICloneable
    {
        private FileSystemWatcher fsw;

        public PresetDropdown(IServiceProvider ServiceProvider, string OwnerName, T DefaultPreset, XmlAttributeOverrides XmlAttributeOverrides)
        {
            InitializeComponent();
            Services = ServiceProvider;
            this.OwnerName = OwnerName;

            defaultPreset = current = DefaultPreset;

            xao = XmlAttributeOverrides;

            this.SetStyle(
                ControlStyles.FixedHeight
                | ControlStyles.Selectable
                | ControlStyles.ResizeRedraw,
                true);

            PopulateDropdown();
        }

        void fsw_Event(object sender, EventArgs e)
        {
            // this is raised on a thread pool thread, so don't touch anything here. just hand off
            // to the UI thread.
            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }

            Action action = delegate
            {
                if (!EventsSuspended && !IsDisposed)
                {
                    SuspendEvents();
                    string name = CurrentName;
                    T preset = current;
                    PopulateDropdown();
                    current = preset;
                    SetPresetByName(name);
                    ResumeEvents();
                }
            };
            try
            {
                this.BeginInvoke(action);
            }
            catch { }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            var dir = GetPresetDir();
            if (dir != null && fsw == null)
            {
                fsw = new FileSystemWatcher(dir, "*.xml");
                fsw.Changed += new FileSystemEventHandler(fsw_Event);
                fsw.Created += new FileSystemEventHandler(fsw_Event);
                fsw.Deleted += new FileSystemEventHandler(fsw_Event);
                fsw.Renamed += new RenamedEventHandler(fsw_Event);
                fsw.EnableRaisingEvents = true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && fsw != null)
            {
                fsw.Dispose();
                fsw = null;
            }
            base.Dispose(disposing);
        }

        private void PopulateDropdown()
        {
            SuspendEvents();
            comboBox.SuspendLayout();
            comboBox.Items.Clear();
            comboBox.Items.Add(new PresetDropdownItem<T>("Default", defaultPreset));
            comboBox.Items.Add(new PresetDropdownItem<T>("Custom", current));
            comboBox.Items.Add(new PresetDropdownItem<T>());
            comboBox.Items.AddRange(LoadPresets());
            comboBox.Items.Add(new PresetDropdownItem<T>());
            comboBox.Items.Add(new PresetDropdownItem<T>("Save current as preset...", SavePreset));
            comboBox.Items.Add(new PresetDropdownItem<T>("Manage presets...", ManagePresets));
            comboBox.SelectedIndex = 0;
            comboBox.ResumeLayout();
            ResumeEvents();
        }

        public PresetDropdownItem<T> this[int index]
        {
            get
            {
                return (PresetDropdownItem<T>)comboBox.Items[index];
            }
        }

        private void SavePreset()
        {
            string name;
            if (InputBox.Show(this, "Type a name for the new preset.", null, null, Path.GetInvalidFileNameChars(), InputBox.ValidationMode.Blacklist, out name) == DialogResult.OK)
            {
                // the input box only filters typed characters, not pasted ones
                name = name.StripIllegalPathChars().Trim();
                if (name == "") name = "Untitled Preset";

                // presets are found by name, and this one is the unsaved settings
                if (name.ToUpperInvariant() == "CUSTOM")
                {
                    MessageBox.Show(this, "\"Custom\" can't be used as a preset name. Please choose a different name.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var dir = GetPresetDir();
                if (dir == null)
                {
                    MessageBox.Show(this, "Error saving preset:\n\nCouldn't access Preset directory", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // not Path.ChangeExtension, which would cut the name off at its last '.'
                var path = Path.Combine(dir, name + ".xml");
                if (!File.Exists(path)
                    || MessageBox.Show(
                        this,
                        "The file already exists. Would you like to replace the file with the new preset?",
                        "",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    FileStream fs = null;
                    SuspendEvents();
                    try
                    {
                        fs = new FileStream(path, FileMode.Create);
                        xmlSerializer.Serialize(fs, current);
                        fs.Close();
                        T preset = current;
                        PopulateDropdown();
                        current = preset;
                        SetPresetByName(name);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(this, "Error saving preset:\n\n" + e.ToString(), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        if (fs != null)
                        {
                            fs.Dispose();
                        }
                        ResumeEvents();
                    }
                }
            }
        }

        private void ManagePresets()
        {
            var dir = GetPresetDir();
            if (dir != null)
            {
                ((PaintDotNet.AppModel.IShellService)Services.GetService(typeof(PaintDotNet.AppModel.IShellService))).LaunchFolder(this, dir);
            }
        }

        private PresetDropdownItem<T>[] LoadPresets()
        {
            var ret = new List<PresetDropdownItem<T>>();
            var invalid = new List<PresetDropdownItem<T>>();
            var dir = GetPresetDir();
            if (dir == null)
            {
                return ret.ToArray();
            }
            foreach (string file in Directory.GetFiles(dir, "*.xml"))
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                    T t = LoadPreset(fs);
                    if (t != null)
                    {
                        string name = Path.GetFileNameWithoutExtension(file);
                        if (name.ToUpperInvariant() == "DEFAULT")
                        {
                            defaultPreset = t;
                            comboBox.Items.RemoveAt(0);
                            comboBox.Items.Insert(0, new PresetDropdownItem<T>("Default", defaultPreset));
                        }
                        else if (name.ToUpperInvariant() != "CUSTOM")
                        {
                            ret.Add(new PresetDropdownItem<T>(name, t));
                        }
                    }
                }
                catch (Exception e)
                {
                    // list it anyway, so a broken preset doesn't just vanish. selecting it says why.
                    string filename = Path.GetFileName(file);
                    string message = e.InnerException != null ? e.Message + "\n" + e.InnerException.Message : e.Message;
                    invalid.Add(new PresetDropdownItem<T>(
                        Path.GetFileNameWithoutExtension(file) + " (couldn't load)",
                        delegate
                        {
                            MessageBox.Show(this, "The preset file \"" + filename + "\" couldn't be loaded:\n\n" + message, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }));
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Dispose();
                    }
                }
            }

            ret.AddRange(invalid);
            return ret.ToArray();
        }

        private T LoadPreset(Stream stream)
        {
            var retval = (T)xmlSerializer.Deserialize(stream);
            return retval;
        }

        object lastsel;
        void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = comboBox.SelectedItem as PresetDropdownItem<T>;
            if (item == null)
            {
                return;
            }

            if (EventsSuspended)
            {
                // keep track of selections made in code, so there's something valid to go back to
                // after a command
                if (item.Type == PresetDropdownItem<T>.ItemType.Preset)
                {
                    lastsel = item;
                }
            }
            else
            {
                SuspendEvents();
                switch (item.Type)
                {
                    case PresetDropdownItem<T>.ItemType.Separator:
                    case PresetDropdownItem<T>.ItemType.Command:
                        if (item.Action != null)
                        {
                            item.Action();
                        }
                        // the command may have selected something itself (saving a preset does)
                        if (comboBox.SelectedItem == item)
                        {
                            comboBox.SelectedItem = lastsel;
                        }
                        break;
                    case PresetDropdownItem<T>.ItemType.Preset:
                        OnPresetChanged(/*item*/);
                        break;
                }
                lastsel = comboBox.SelectedItem;
                ResumeEvents();
            }
        }

        //private void OnPresetChanged(PresetDropdownItem<T> item)
        //{
        //    var handler = PresetChanged;
        //    if (handler != null)
        //    {
        //        T newpreset = default(T);
        //        if (item.Name == "Custom")
        //        {
        //            newpreset = current;
        //        }
        //        else if (item.Preset != null)
        //        {
        //            newpreset = (T)item.Preset.Clone();
        //        }
        //        current = newpreset;
        //        var args = new PresetChangedEventArgs<T>(item.Name, newpreset);
        //        handler(this, args);
        //    }
        //}

        public void OnPresetChanged()
        {
            var item = comboBox.SelectedItem as PresetDropdownItem<T>;
            if (item != null)
            {
                var handler = PresetChanged;
                if (handler != null)
                {
                    T newpreset = default(T);
                    if (item.Name == "Custom")
                    {
                        newpreset = current;
                    }
                    else if (item.Preset != null)
                    {
                        newpreset = (T)item.Preset.Clone();
                    }
                    current = newpreset;
                    var args = new PresetChangedEventArgs<T>(item.Name, newpreset);
                    handler(this, args);
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool handled = false;
            if (!EventsSuspended)
            {
                int index = comboBox.SelectedIndex;
                switch (keyData)
                {
                    case Keys.Up:
                        handled = true;
                        if (index > 0)
                        {
                            switch (((PresetDropdownItem<T>)comboBox.Items[index - 1]).Type)
                            {
                                case PresetDropdownItem<T>.ItemType.Separator:
                                    comboBox.SelectedIndex -= 2;
                                    break;
                                default:
                                    comboBox.SelectedIndex--;
                                    break;
                            }
                        }
                        break;
                    case Keys.Down:
                        handled = true;
                        if (index < comboBox.Items.Count - 1)
                        {
                            switch (((PresetDropdownItem<T>)comboBox.Items[index + 1]).Type)
                            {
                                case PresetDropdownItem<T>.ItemType.Separator:
                                    var itemtogoto = (PresetDropdownItem<T>)comboBox.Items[index + 2];
                                    if (itemtogoto.Type == PresetDropdownItem<T>.ItemType.Preset)
                                        comboBox.SelectedIndex += 2;
                                    break;
                                case PresetDropdownItem<T>.ItemType.Preset:
                                    comboBox.SelectedIndex++;
                                    break;
                            }
                        }
                        break;
                }
            }
            return handled || base.ProcessCmdKey(ref msg, keyData);
        }

        public void DefaultMeasureItem(object sender, MeasureItemEventArgs e)
        {
            switch (drawMode)
            {
                case DrawMode.Normal:
                case DrawMode.OwnerDrawFixed:
                    e.ItemHeight = comboBox.ItemHeight;
                    break;
                case DrawMode.OwnerDrawVariable:
                    if (MeasureItem != null)
                    {
                        MeasureItem(sender, e);
                    }
                    break;
            }
        }

        public void DefaultDrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0 && e.Index < comboBox.Items.Count)
            {
                var item = comboBox.Items[e.Index] as PresetDropdownItem<T>;
                switch (drawMode)
                {
                    case DrawMode.Normal:
                        switch (item.Type)
                        {
                            case PresetDropdownItem<T>.ItemType.Separator:
                                float midy = (e.Bounds.Top + e.Bounds.Bottom) / 2f;
                                using (Pen pen = new Pen(SystemColors.Highlight))
                                {
                                    e.Graphics.DrawLine(
                                        pen,
                                        e.Bounds.Left + 5,
                                        midy,
                                        e.Bounds.Right - 5,
                                        midy);
                                }
                                break;

                            default:
                                e.DrawBackground();
                                e.DrawFocusRectangle();
                                using (SolidBrush brush = new SolidBrush(e.ForeColor))
                                {
                                    e.Graphics.DrawString(
                                        item.Name,
                                        comboBox.Font,
                                        brush,
                                        e.Bounds);
                                }
                                break;
                        }
                        break;
                    case DrawMode.OwnerDrawFixed:
                    case DrawMode.OwnerDrawVariable:
                        if (DrawItem != null)
                        {
                            DrawItem(sender, e);
                        }
                        else
                        {
                            goto case DrawMode.Normal;
                        }
                        break;
                }
            }
        }

        public string GetPresetDir()
        {
            try
            {
                var retval = Path.Combine(Path.Combine(
                            ((PaintDotNet.AppModel.IUserFilesService)Services.GetService(typeof(PaintDotNet.AppModel.IUserFilesService))).UserFilesPath,
                            "Effect Presets"),
                            OwnerName);
                if (!Directory.Exists(retval))
                    Directory.CreateDirectory(retval);
                return retval;
            }
            catch
            {
                return null;
            }
        }

        private IServiceProvider Services;

        public string OwnerName
        { get; private set; }

        private T defaultPreset;
        public T Default { get { return (T)defaultPreset.Clone(); } }

        private T current;
        public T Current
        {
            get
            {
                return current;
            }
            set
            {
                SwitchToCustom();
                current = value;
            }
        }

        public string CurrentName
        {
            get
            {
                var item = comboBox.SelectedItem as PresetDropdownItem<T>;
                if (item != null)
                {
                    return item.Name;
                }
                else { return null; }
            }
        }

        private XmlSerializer xs;
        private XmlSerializer xmlSerializer
        {
            get
            {
                if (xs == null)
                {
                    xs = new XmlSerializer(typeof(T), XmlAttributeOverrides, new Type[] { typeof(Color) }, null, null);
                }
                return xs;
            }
        }

        private XmlAttributeOverrides xao;
        public XmlAttributeOverrides XmlAttributeOverrides
        {
            get
            {
                return xao;
            }
        }

        private int suspendcount = 0;
        private bool EventsSuspended
        {
            get
            {
                return suspendcount > 0;
            }
        }

        private void SuspendEvents()
        {
            suspendcount++;
        }

        private void ResumeEvents()
        {
            suspendcount--;
        }

        private void SwitchToCustom()
        {
            SuspendEvents();
            if (comboBox.Items.Count > 1)
            {
                comboBox.SelectedIndex = 1;
            }
            ResumeEvents();
        }

        private DrawMode drawMode = DrawMode.Normal;
        public DrawMode DrawMode
        {
            get { return drawMode; }
            set
            {
                drawMode = value;
                comboBox.DrawMode = DrawMode.OwnerDrawVariable;
            }
        }

        public event PresetChangedEventHandler<T> PresetChanged;
        public event MeasureItemEventHandler MeasureItem;
        public event DrawItemEventHandler DrawItem;

        public void SetPresetByName(string name)
        {
            foreach (PresetDropdownItem<T> item in comboBox.Items)
            {
                if (item.Type == PresetDropdownItem<T>.ItemType.Preset
                    && item.Name == name)
                {
                    comboBox.SelectedItem = item;
                    return;
                }
            }
            if (name != null)
                SwitchToCustom();
        }

        public void AddPreset(T preset, string name)
        {
            var filename = name + ".xml";
            var dir = GetPresetDir();
            if (dir != null)
            {
                var path = Path.Combine(dir, filename);
                if (!File.Exists(path))
                {
                    using (FileStream fs = new FileStream(path, FileMode.CreateNew))
                    {
                        xmlSerializer.Serialize(fs, preset);
                    }
                    PopulateDropdown();
                }
            }
        }

        private const string installedDefaultsFileName = "installed-defaults.txt";

        /// <summary>
        /// Adds a built-in preset the first time it's seen. The names already added are kept in a
        /// list in the preset directory, so a built-in preset the user deletes stays deleted.
        /// </summary>
        public void AddDefaultPreset(T preset, string name)
        {
            var dir = GetPresetDir();
            if (dir != null)
            {
                try
                {
                    var listPath = Path.Combine(dir, installedDefaultsFileName);
                    if (!File.Exists(listPath) || Array.IndexOf(File.ReadAllLines(listPath), name) < 0)
                    {
                        AddPreset(preset, name);
                        File.AppendAllLines(listPath, new string[] { name });
                    }
                }
                catch { }
            }
        }

        public void AddPreset(Stream stream, string name)
        {
            AddPreset(LoadPreset(stream), name);
        }

        private void InitializeComponent()
        {
            this.comboBox = new ComboBox();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox.FormattingEnabled = true;
            this.comboBox.Location = new System.Drawing.Point(0, 0);
            this.comboBox.Margin = new System.Windows.Forms.Padding(0);
            this.comboBox.Name = "comboBox1";
            this.comboBox.Size = new System.Drawing.Size(100, 21);
            this.comboBox.TabIndex = 0;
            this.comboBox.DrawItem += new DrawItemEventHandler(DefaultDrawItem);
            this.comboBox.MeasureItem += new MeasureItemEventHandler(DefaultMeasureItem);
            this.comboBox.SelectedIndexChanged += new System.EventHandler(comboBox_SelectedIndexChanged);
            this.comboBox.DropDownHeight = int.MaxValue;
            // 
            // PresetDropdown
            // 
            this.Controls.Add(this.comboBox);
            this.Name = "PresetDropdown";
            this.Size = new System.Drawing.Size(100, 21);
            this.ResumeLayout(false);
        }

        protected override void OnSizeChanged(System.EventArgs e)
        {
            this.Height = this.comboBox.Height;

            base.OnSizeChanged(e);
        }

        private System.Windows.Forms.ComboBox comboBox;
    }
}