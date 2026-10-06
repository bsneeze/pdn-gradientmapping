using PaintDotNet;
using pyrochild.effects.common;

namespace pyrochild.effects.gradientmapping.tests
{
    public class GradientMapTests
    {
        // output alpha equals the lookup index, so tests can read back which index a pixel mapped to
        private static Gradient IndexGradient()
        {
            var gradient = new Gradient();
            gradient.Add(0, ColorBgra.FromBgra(0, 0, 0, 0));
            gradient.Add(1, ColorBgra.FromBgra(0, 0, 0, 255));
            return gradient;
        }

        private static int Index(Channel channel, ColorBgra color, bool wrap = true, int offset = 0)
        {
            var map = new UnaryPixelHistogramOps.GradientMap(IndexGradient(), channel, wrap, false, offset);
            return map.Apply(color).A;
        }

        [Fact]
        public void IndexGradient_MapsEveryIndexToItself()
        {
            for (int i = 0; i < 256; i++)
            {
                Assert.Equal(i, Index(Channel.R, ColorBgra.FromBgra(0, 0, (byte)i, 255)));
            }
        }

        [Fact]
        public void Apply_RgbaChannels_UseThatChannel()
        {
            ColorBgra color = ColorBgra.FromBgra(10, 20, 30, 40);

            Assert.Equal(10, Index(Channel.B, color));
            Assert.Equal(20, Index(Channel.G, color));
            Assert.Equal(30, Index(Channel.R, color));
            Assert.Equal(40, Index(Channel.A, color));
        }

        [Fact]
        public void Apply_CmykChannels()
        {
            ColorBgra color = ColorBgra.FromBgra(50, 100, 200, 255);

            Assert.Equal(0, Index(Channel.C, color));
            Assert.Equal(100, Index(Channel.M, color));
            Assert.Equal(150, Index(Channel.Y, color));
            Assert.Equal(55, Index(Channel.K, color));
        }

        [Fact]
        public void Apply_Value_IsTheLargestChannel()
        {
            Assert.Equal(200, Index(Channel.V, ColorBgra.FromBgra(50, 100, 200, 255)));
            Assert.Equal(0, Index(Channel.V, ColorBgra.Black));
        }

        [Fact]
        public void Apply_Saturation()
        {
            Assert.Equal(0, Index(Channel.S, ColorBgra.Black));
            Assert.Equal(0, Index(Channel.S, ColorBgra.FromBgra(128, 128, 128, 255)));
            Assert.Equal(255, Index(Channel.S, ColorBgra.Red));
            Assert.Equal(127, Index(Channel.S, ColorBgra.FromBgra(100, 100, 200, 255)));
        }

        [Fact]
        public void Apply_Hue_PrimariesAreAThirdOfTheRangeApart()
        {
            Assert.Equal(0, Index(Channel.H, ColorBgra.Red));
            Assert.Equal(42, Index(Channel.H, ColorBgra.Yellow));
            Assert.Equal(85, Index(Channel.H, ColorBgra.Lime));
            Assert.Equal(127, Index(Channel.H, ColorBgra.Cyan));
            Assert.Equal(170, Index(Channel.H, ColorBgra.Blue));
            Assert.Equal(213, Index(Channel.H, ColorBgra.Magenta));
        }

        [Fact]
        public void Apply_Hue_BetweenMagentaAndRed_IsAtTheTopOfTheRange()
        {
            int hue = Index(Channel.H, ColorBgra.FromBgra(128, 0, 255, 255));

            Assert.InRange(hue, 230, 240);
        }

        [Fact]
        public void Apply_Hue_OfGray_IsZero()
        {
            Assert.Equal(0, Index(Channel.H, ColorBgra.FromBgra(128, 128, 128, 255)));
        }

        [Fact]
        public void Apply_Luminosity()
        {
            Assert.Equal(0, Index(Channel.L, ColorBgra.Black));
            Assert.Equal(255, Index(Channel.L, ColorBgra.White));
            Assert.Equal(128, Index(Channel.L, ColorBgra.FromBgra(128, 128, 128, 255)));
            Assert.True(Index(Channel.L, ColorBgra.Lime) > Index(Channel.L, ColorBgra.Red));
            Assert.True(Index(Channel.L, ColorBgra.Red) > Index(Channel.L, ColorBgra.Blue));
        }

        [Fact]
        public void Apply_OffsetWithWrap_WrapsAround()
        {
            Assert.Equal(4, Index(Channel.R, ColorBgra.FromBgra(0, 0, 250, 255), wrap: true, offset: 10));
            Assert.Equal(251, Index(Channel.R, ColorBgra.FromBgra(0, 0, 5, 255), wrap: true, offset: -10));
        }

        [Fact]
        public void Apply_OffsetWithoutWrap_Clamps()
        {
            Assert.Equal(255, Index(Channel.R, ColorBgra.FromBgra(0, 0, 250, 255), wrap: false, offset: 10));
            Assert.Equal(0, Index(Channel.R, ColorBgra.FromBgra(0, 0, 5, 255), wrap: false, offset: -10));
            Assert.Equal(110, Index(Channel.R, ColorBgra.FromBgra(0, 0, 100, 255), wrap: false, offset: 10));
        }

        [Fact]
        public void Apply_LockAlpha_KeepsTheSourceAlpha()
        {
            var gradient = new Gradient();
            gradient.Add(0, ColorBgra.Red);
            gradient.Add(1, ColorBgra.Red);
            ColorBgra source = ColorBgra.FromBgra(1, 2, 3, 77);

            var locked = new UnaryPixelHistogramOps.GradientMap(gradient, Channel.L, true, true, 0);
            var unlocked = new UnaryPixelHistogramOps.GradientMap(gradient, Channel.L, true, false, 0);

            Assert.Equal(ColorBgra.FromBgra(0, 0, 255, 77), locked.Apply(source));
            Assert.Equal(ColorBgra.Red, unlocked.Apply(source));
        }

        [Fact]
        public void Token_BuildsItsMapFromItsCurrentSettings()
        {
            var token = new ConfigToken { Gradient = IndexGradient(), InputChannel = Channel.R };
            ColorBgra color = ColorBgra.FromBgra(0, 0, 100, 255);

            Assert.Equal(100, token.Uop.Apply(color).A);

            token.Offset = 20;

            Assert.Equal(120, token.Uop.Apply(color).A);
        }
    }
}
