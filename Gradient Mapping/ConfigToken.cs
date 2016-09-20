using PaintDotNet;
using PaintDotNet.Effects;
using pyrochild.effects.common;

namespace pyrochild.effects.gradientmapping
{
    public class ConfigToken : EffectConfigToken
    {
        private Gradient gGradient;
        public Gradient Gradient
        {
            get { return gGradient; }
            set
            {
                gGradient = value;
                uop = null;
            }
        }

        private Channel inputchannel;
        public Channel InputChannel
        {
            get { return inputchannel; }
            set
            {
                inputchannel = value;
                uop = null;
            }
        }

        private int iOffset;
        public int Offset
        {
            get { return iOffset; }
            set
            {
                iOffset = value;
                uop = null;
            }
        }

        private bool bWrap;
        public bool Wrap
        {
            get { return bWrap; }
            set
            {
                bWrap = value;
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

        public string Preset { get; set; }

        private UnaryPixelOp MakeUop()
        {
            return new UnaryPixelHistogramOps.GradientMap(Gradient, InputChannel, Wrap, LockAlpha, Offset);
        }

        public bool LockAlpha { get; set; }

        public ConfigToken()
        {
            Preset = "Default";
            inputchannel = Channel.L;
            bWrap = true;
            LockAlpha = false;
            iOffset = 0;
            gGradient = new Gradient();
            gGradient.SetDefault();
            gGradient.Spread();
        }

        public ConfigToken(ConfigToken toCopy)
        {
            Preset = toCopy.Preset;
            inputchannel = toCopy.inputchannel;
            bWrap = toCopy.bWrap;
            LockAlpha = toCopy.LockAlpha;
            iOffset = toCopy.iOffset;
            gGradient = (Gradient)toCopy.gGradient.Clone();
        }

        public override object Clone()
        {
            return new ConfigToken(this);
        }
    }
}