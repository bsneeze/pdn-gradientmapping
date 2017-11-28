using PaintDotNet;
using PaintDotNet.Effects;
using System.Drawing;

namespace pyrochild.effects.gradientmapping
{
    [PluginSupportInfo(typeof(PluginSupportInfo))]
    [EffectCategory(EffectCategory.Adjustment)]
    class GradientMapping : Effect
    {
        public GradientMapping()
            : base(StaticName, new Bitmap(typeof(GradientMapping), "icon.png"), null, EffectFlags.Configurable)
        {
        }

        public static string StaticName
        {
            get
            {
                string s = "Gradient Mapping";
#if DEBUG
                s += " BETA";
#endif
                return s;
            }
        }

        public static string StaticDialogName
        {
            get
            {
                return StaticName + " by pyrochild";
            }
        }

        public override EffectConfigDialog CreateConfigDialog()
        {
            return new ConfigDialog();
        }

        public override void Render(EffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs, Rectangle[] rois, int startIndex, int length)
        {
            ConfigToken token = parameters as ConfigToken;

            if (token.Gradient.Count == 0) return;

            UnaryPixelOp uop = token.Uop;

            for (int i = startIndex; i < startIndex + length; ++i)
            {
                uop.Apply(dstArgs.Surface, srcArgs.Surface, rois[i]);
            }
        }
    }
}