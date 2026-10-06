using PaintDotNet;
using pyrochild.effects.common;
using System;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;

namespace pyrochild.effects.gradientmapping.tests
{
    public class ConfigTokenTests
    {
        // same as PresetDropdown's
        private static readonly XmlSerializer serializer =
            new XmlSerializer(typeof(ConfigToken), ConfigToken.GetXao(), new Type[] { typeof(Color) }, null, null);

        private static ConfigToken Load(string xml)
        {
            using (var reader = new StringReader(xml))
            {
                return (ConfigToken)serializer.Deserialize(reader)!;
            }
        }

        private static string Save(ConfigToken token)
        {
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, token);
                return writer.ToString();
            }
        }

        private static string Preset(string body, string attributes = "InputChannel=\"L\" Offset=\"0\" Wrap=\"true\" LockAlpha=\"false\"")
        {
            return "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n"
                + "<Gradient xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " + attributes + ">\n"
                + body
                + "</Gradient>";
        }

        // the preset from the original bug report: colors bunched together well away from 0
        private const string ClusteredColors = @"
  <Colors>
    <ColorBgra B=""0"" G=""0"" R=""0"" A=""0"" />
    <ColorBgra B=""76"" G=""91"" R=""0"" A=""255"" />
    <ColorBgra B=""76"" G=""91"" R=""0"" A=""255"" />
    <ColorBgra B=""0"" G=""187"" R=""255"" A=""255"" />
    <ColorBgra B=""0"" G=""187"" R=""255"" A=""255"" />
    <ColorBgra B=""76"" G=""91"" R=""0"" A=""255"" />
    <ColorBgra B=""99"" G=""181"" R=""0"" A=""255"" />
  </Colors>";

        private const string ClusteredPositions = @"
  <Positions>
    <double>0.4444444477558136</double>
    <double>0.44744744896888733</double>
    <double>0.5735735893249512</double>
    <double>0.5765765905380249</double>
    <double>0.6126126050949097</double>
    <double>0.6216216087341309</double>
    <double>0.6246246099472046</double>
  </Positions>";

        private static readonly ColorBgra[] clusteredColors =
        {
            ColorBgra.FromBgra(0, 0, 0, 0),
            ColorBgra.FromBgra(76, 91, 0, 255),
            ColorBgra.FromBgra(76, 91, 0, 255),
            ColorBgra.FromBgra(0, 187, 255, 255),
            ColorBgra.FromBgra(0, 187, 255, 255),
            ColorBgra.FromBgra(76, 91, 0, 255),
            ColorBgra.FromBgra(99, 181, 0, 255),
        };

        private static readonly double[] clusteredPositions =
        {
            0.4444444477558136,
            0.44744744896888733,
            0.5735735893249512,
            0.5765765905380249,
            0.6126126050949097,
            0.6216216087341309,
            0.6246246099472046,
        };

        [Fact]
        public void Load_ClusteredColors_KeepsEachColorAtItsPosition()
        {
            ConfigToken token = Load(Preset(ClusteredColors + ClusteredPositions));

            Assert.Equal(clusteredColors, token.Colors);
            Assert.Equal(clusteredPositions, token.Positions);
        }

        [Fact]
        public void Load_ClusteredColors_SurvivesAResave()
        {
            ConfigToken token = Load(Save(Load(Preset(ClusteredColors + ClusteredPositions))));

            Assert.Equal(clusteredColors, token.Colors);
            Assert.Equal(clusteredPositions, token.Positions);
        }

        [Fact]
        public void Load_PositionsBeforeColors_GivesTheSameGradient()
        {
            ConfigToken token = Load(Preset(ClusteredPositions + ClusteredColors));

            Assert.Equal(clusteredColors, token.Colors);
            Assert.Equal(clusteredPositions, token.Positions);
        }

        [Fact]
        public void Load_ColorsWithoutPositions_SpreadsThemEvenly()
        {
            ConfigToken token = Load(Preset(@"
  <Colors>
    <ColorBgra B=""0"" G=""0"" R=""255"" A=""255"" />
    <ColorBgra B=""0"" G=""255"" R=""0"" A=""255"" />
    <ColorBgra B=""255"" G=""0"" R=""0"" A=""255"" />
  </Colors>"));

            Assert.Equal(new[] { ColorBgra.Red, ColorBgra.Lime, ColorBgra.Blue }, token.Colors);
            Assert.Equal(new[] { 0.0, 0.5, 1.0 }, token.Positions);
        }

        [Fact]
        public void Load_FewerColorsThanTheDefaultGradient_DropsTheDefaultColors()
        {
            ConfigToken token = Load(Preset(@"
  <Colors>
    <ColorBgra B=""0"" G=""0"" R=""255"" A=""255"" />
  </Colors>
  <Positions>
    <double>0.25</double>
  </Positions>"));

            Assert.Equal(new[] { ColorBgra.Red }, token.Colors);
            Assert.Equal(new[] { 0.25 }, token.Positions);
        }

        [Fact]
        public void Load_UnsortedPositions_SortsColorsWithThem()
        {
            ConfigToken token = Load(Preset(@"
  <Colors>
    <ColorBgra B=""0"" G=""0"" R=""255"" A=""255"" />
    <ColorBgra B=""0"" G=""255"" R=""0"" A=""255"" />
    <ColorBgra B=""255"" G=""0"" R=""0"" A=""255"" />
  </Colors>
  <Positions>
    <double>0.9</double>
    <double>0.1</double>
    <double>0.5</double>
  </Positions>"));

            Assert.Equal(new[] { ColorBgra.Lime, ColorBgra.Blue, ColorBgra.Red }, token.Colors);
            Assert.Equal(new[] { 0.1, 0.5, 0.9 }, token.Positions);
        }

        [Fact]
        public void Load_ReadsTheSettingsAttributes()
        {
            ConfigToken token = Load(Preset("", "InputChannel=\"S\" Offset=\"-40\" Wrap=\"false\" LockAlpha=\"true\""));

            Assert.Equal(Channel.S, token.InputChannel);
            Assert.Equal(-40, token.Offset);
            Assert.False(token.Wrap);
            Assert.True(token.LockAlpha);
        }

        [Fact]
        public void Load_NoColors_KeepsTheDefaultGradient()
        {
            ConfigToken token = Load(Preset(""));

            Assert.Equal(new[] { ColorBgra.Black, ColorBgra.White }, token.Colors);
            Assert.Equal(new[] { 0.0, 1.0 }, token.Positions);
        }

        [Fact]
        public void Save_WritesColorsAsAttributesAndLeavesOutThePresetName()
        {
            var token = new ConfigToken { Preset = "Rainbow" };

            string xml = Save(token);

            Assert.Contains("<ColorBgra B=\"0\" G=\"0\" R=\"0\" A=\"255\" />", xml);
            Assert.DoesNotContain("Bgra=", xml);
            Assert.DoesNotContain("Rainbow", xml);
        }

        [Fact]
        public void SaveThenLoad_RoundTripsEverything()
        {
            var gradient = new Gradient();
            gradient.Add(0.2, ColorBgra.FromBgra(1, 2, 3, 4));
            gradient.Add(0.2, ColorBgra.FromBgra(5, 6, 7, 8));
            gradient.Add(1.0 / 3.0, ColorBgra.FromBgra(9, 10, 11, 12));
            var token = new ConfigToken
            {
                Gradient = gradient,
                InputChannel = Channel.H,
                Offset = 17,
                Wrap = false,
                LockAlpha = true,
            };

            ConfigToken loaded = Load(Save(token));

            Assert.Equal(token.Colors, loaded.Colors);
            Assert.Equal(token.Positions, loaded.Positions);
            Assert.Equal(Channel.H, loaded.InputChannel);
            Assert.Equal(17, loaded.Offset);
            Assert.False(loaded.Wrap);
            Assert.True(loaded.LockAlpha);
        }

        [Fact]
        public void Clone_CopiesTheGradientInsteadOfSharingIt()
        {
            var token = new ConfigToken { Preset = "Rainbow", Offset = 5 };

            var clone = (ConfigToken)token.Clone();
            clone.Gradient.SetColor(0, ColorBgra.Red);

            Assert.Equal("Rainbow", clone.Preset);
            Assert.Equal(5, clone.Offset);
            Assert.Equal(ColorBgra.Black, token.Gradient.GetColor(0));
        }
    }
}
