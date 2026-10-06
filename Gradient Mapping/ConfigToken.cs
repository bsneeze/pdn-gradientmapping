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
                if (positionsSet)
                {
                    // Positions came first in the file. keep them and just fill in the colors.
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (i < gradient.Count)
                        {
                            gradient.SetColor(i, value[i]);
                        }
                        else
                        {
                            gradient.Add(1, value[i]);
                        }
                    }
                    return;
                }

                if (value.Length < gradient.Count) gradient.Clear();

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
                gradient.SetPositions(value);
                positionsSet = true;
            }
        }

        // set while loading from XML, so Colors knows not to spread the colors out evenly
        private bool positionsSet;

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

        public static XmlAttributeOverrides GetXao()
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

        public override object Clone()
        {
            return new ConfigToken(this);
        }
    }
}