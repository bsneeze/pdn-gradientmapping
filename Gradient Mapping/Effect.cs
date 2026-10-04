using PaintDotNet;
using PaintDotNet.Effects;
using PaintDotNet.Imaging;
using PaintDotNet.Rendering;
using pyrochild.effects.common;
using System.Drawing;

namespace pyrochild.effects.gradientmapping
{
    [PluginSupportInfo(typeof(PluginSupportInfo))]
    [EffectCategory(EffectCategory.Adjustment)]
    public class GradientMapping : BitmapEffect<ConfigToken>
    {
        UnaryPixelOp uop;
        bool hasGradient;

        public GradientMapping()
            : base(StaticName, new Bitmap(typeof(GradientMapping), "icon.png"), null, BitmapEffectOptions.Create() with { IsConfigurable = true })
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

        protected override IEffectConfigForm OnCreateConfigForm()
        {
            return new ConfigDialog();
        }

        protected override void OnSetToken(ConfigToken newToken)
        {
            base.OnSetToken(newToken);

            if (newToken != null)
            {
                uop = newToken.Uop;
                hasGradient = newToken.Gradient.Count > 0;
            }
        }

        protected override unsafe void OnRender(IBitmapEffectOutput output)
        {
            if (uop == null || !hasGradient)
            {
                return;
            }

            RectInt32 bounds = output.Bounds;

            using (IEffectInputBitmap<ColorBgra32> srcBitmap = Environment.GetSourceBitmapBgra32())
            using (IBitmapLock<ColorBgra32> srcLock = srcBitmap.Lock(new RectInt32(0, 0, srcBitmap.Size)))
            using (IBitmapLock<ColorBgra32> dstLock = output.LockBgra32())
            {
                RegionPtr<ColorBgra32> srcRegion = new RegionPtr<ColorBgra32>(srcLock.Buffer, srcLock.Size, srcLock.BufferStride);
                RegionPtr<ColorBgra32> dstRegion = new RegionPtr<ColorBgra32>(dstLock.Buffer, dstLock.Size, dstLock.BufferStride);

                uop.Apply(dstRegion, srcRegion, bounds);
            }
        }
    }
}