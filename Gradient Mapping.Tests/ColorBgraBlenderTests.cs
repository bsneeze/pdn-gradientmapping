using PaintDotNet;
using pyrochild.effects.common;

namespace pyrochild.effects.gradientmapping.tests
{
    public class ColorBgraBlenderTests
    {
        private static readonly ColorBgra teal = ColorBgra.FromBgra(76, 91, 0, 255);

        [Fact]
        public void Blend_AtTheEnds_ReturnsEachColor()
        {
            ColorBgra a = ColorBgra.FromBgra(10, 20, 30, 40);
            ColorBgra b = ColorBgra.FromBgra(200, 150, 100, 250);

            Assert.Equal(a, ColorBgraBlender.Blend(a, b, 0.0));
            Assert.Equal(b, ColorBgraBlender.Blend(a, b, 1.0));
        }

        [Fact]
        public void Blend_OpaqueColors_StaysOpaque()
        {
            Assert.Equal(255, ColorBgraBlender.Blend(ColorBgra.Red, ColorBgra.Blue, 0.3).A);
        }

        [Fact]
        public void Blend_IsSymmetric()
        {
            Assert.Equal(
                ColorBgraBlender.Blend(ColorBgra.Red, ColorBgra.Blue, 0.25),
                ColorBgraBlender.Blend(ColorBgra.Blue, ColorBgra.Red, 0.75));
        }

        [Theory]
        [InlineData(0.1)]
        [InlineData(0.5)]
        [InlineData(0.9)]
        public void Blend_WithATransparentColor_KeepsTheOtherColorsRgb(double blend)
        {
            ColorBgra color = ColorBgraBlender.Blend(ColorBgra.FromBgra(0, 0, 0, 0), teal, blend);

            Assert.Equal(teal.B, color.B);
            Assert.Equal(teal.G, color.G);
            Assert.Equal(teal.R, color.R);
        }

        [Fact]
        public void Blend_WithATransparentColor_StillFadesAlphaLinearly()
        {
            Assert.Equal(128, ColorBgraBlender.Blend(ColorBgra.FromBgra(0, 0, 0, 0), teal, 0.5).A);
            Assert.Equal(64, ColorBgraBlender.Blend(ColorBgra.FromBgra(0, 0, 0, 0), teal, 0.25).A);
        }

        [Fact]
        public void Blend_TwoTransparentColors_BlendsTheirRgbPlainly()
        {
            ColorBgra color = ColorBgraBlender.Blend(ColorBgra.FromBgra(0, 0, 0, 0), ColorBgra.FromBgra(255, 255, 255, 0), 0.5);

            Assert.Equal(ColorBgra.FromBgra(188, 188, 188, 0), color);
        }

        [Fact]
        public void Blend_WeightsRgbByAlpha()
        {
            ColorBgra faintRed = ColorBgra.FromBgra(0, 0, 255, 51);
            ColorBgra solidBlue = ColorBgra.FromBgra(255, 0, 0, 255);

            ColorBgra color = ColorBgraBlender.Blend(faintRed, solidBlue, 0.5);

            Assert.True(color.B > color.R);
            Assert.Equal(153, color.A);
        }
    }
}
