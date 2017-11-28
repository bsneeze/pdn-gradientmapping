using PaintDotNet;
using PaintDotNet.Effects;
using pyrochild.effects.common;
using System;
using System.Xml.Serialization;

namespace pyrochild.effects.gradientmapping
{
    [Serializable]
    [XmlRoot("Gradient")]
    public class ConfigToken : EffectConfigToken
    {
        private Gradient gradient;
        
        [XmlIgnore]
        public Gradient Gradient
        {
            get { return gradient; }
            set
            {
                gradient = value;
                uop = null;
            }
        }

        private Channel inputChannel;
        public Channel InputChannel
        {
            get { return inputChannel; }
            set
            {
                inputChannel = value;
                uop = null;
            }
        }

        private int offset;
        public int Offset
        {
            get { return offset; }
            set
            {
                offset = value;
                uop = null;
            }
        }

        private bool wrap;
        public bool Wrap
        {
            get { return wrap; }
            set
            {
                wrap = value;
                uop = null;
            }
        }

        private UnaryPixelOp uop;
        public UnaryPixelOp Uop
        {
            get
            {
                if (uop == null)
                {
                    uop = MakeUop();
                }

                return uop;
            }
        }

        [XmlIgnore]
        public string Preset { get; set; }

        private UnaryPixelOp MakeUop()
        {
            return new UnaryPixelHistogramOps.GradientMap(Gradient, InputChannel, Wrap, LockAlpha, Offset);
        }

        public bool LockAlpha { get; set; }

        public ConfigToken()
        {
            Preset = "Default";
            inputChannel = Channel.L;
            wrap = true;
            LockAlpha = false;
            offset = 0;
            gradient = new Gradient();
            gradient.SetDefault();
        }

        public ConfigToken(ConfigToken toCopy)
        {
            Preset = toCopy.Preset;
            inputChannel = toCopy.inputChannel;
            wrap = toCopy.wrap;
            LockAlpha = toCopy.LockAlpha;
            offset = toCopy.offset;
            gradient = (Gradient)toCopy.gradient.Clone();
        }

        public override object Clone()
        {
            return new ConfigToken(this);
        }
    }
}