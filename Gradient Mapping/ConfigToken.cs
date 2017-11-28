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

        public ColorBgra[] Colors
        {
            get
            {
                var retval = new ColorBgra[gradient.Count];
                for (int i = 0; i < gradient.Count; i++)
                {
                    retval[i] = gradient.GetColor(i);
                }
                return retval;
            }
            set
            {
                double space = value.Length > 1 ? 1.0 / (value.Length - 1) : 0;

                for (int i = 0; i < value.Length; i++)
                {
                    if (i < gradient.Count)
                    {
                        gradient.SetColor(i, value[i]);
                        gradient.SetPosition(i, i * space);
                    }
                    else
                    {
                        gradient.Add(i * space, value[i]);
                    }
                }
            }
        }

        public double[] Positions
        {
            get
            {
                var retval = new double[gradient.Count];
                for (int i = 0; i < gradient.Count; i++)
                {
                    retval[i] = gradient.GetPosition(i);
                }
                return retval;
            }
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (i < gradient.Count)
                    {
                        gradient.SetPosition(i, value[i]);
                    }
                    else
                    {
                        gradient.Add(value[i], ColorBgra.Black);
                    }
                }
            }
        }

        private Channel inputChannel;
        [XmlAttribute]
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
        [XmlAttribute]
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
        [XmlAttribute]
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

        [XmlAttribute]
        public bool LockAlpha { get; set; }

        public ConfigToken()
        {
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