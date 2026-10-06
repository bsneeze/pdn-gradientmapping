using PaintDotNet;
using pyrochild.effects.common;
using System;

namespace pyrochild.effects.gradientmapping.tests
{
    public class GradientTests
    {
        private static double[] Positions(Gradient gradient)
        {
            var positions = new double[gradient.Count];
            for (int i = 0; i < gradient.Count; i++)
            {
                positions[i] = gradient.GetPosition(i);
            }
            return positions;
        }

        private static ColorBgra[] Colors(Gradient gradient)
        {
            var colors = new ColorBgra[gradient.Count];
            for (int i = 0; i < gradient.Count; i++)
            {
                colors[i] = gradient.GetColor(i);
            }
            return colors;
        }

        [Fact]
        public void Add_KeepsColorsSortedByPosition()
        {
            var gradient = new Gradient();
            gradient.Add(0.5, ColorBgra.Red);
            gradient.Add(0.1, ColorBgra.Blue);
            gradient.Add(0.9, ColorBgra.White);
            gradient.Add(0.3, ColorBgra.Black);

            Assert.Equal(new[] { 0.1, 0.3, 0.5, 0.9 }, Positions(gradient));
            Assert.Equal(new[] { ColorBgra.Blue, ColorBgra.Black, ColorBgra.Red, ColorBgra.White }, Colors(gradient));
        }

        [Fact]
        public void Add_AtExistingPosition_GoesAfterIt()
        {
            var gradient = new Gradient();
            gradient.Add(0.5, ColorBgra.Red);
            gradient.Add(0.5, ColorBgra.Blue);
            gradient.Add(0.5, ColorBgra.White);

            Assert.Equal(new[] { ColorBgra.Red, ColorBgra.Blue, ColorBgra.White }, Colors(gradient));
        }

        [Fact]
        public void Add_ManyColorsAtOnePosition_KeepsInsertionOrder()
        {
            var gradient = new Gradient();
            for (int i = 0; i < 40; i++)
            {
                gradient.Add(0.5, ColorBgra.FromBgra((byte)i, 0, 0, 255));
            }

            for (int i = 0; i < 40; i++)
            {
                Assert.Equal(i, gradient.GetColor(i).B);
            }
        }

        [Fact]
        public void GetColor_OutsideTheStops_ReturnsTheNearestStop()
        {
            var gradient = new Gradient();
            gradient.Add(0.25, ColorBgra.Red);
            gradient.Add(0.75, ColorBgra.Blue);

            Assert.Equal(ColorBgra.Red, gradient.GetColor(0.0));
            Assert.Equal(ColorBgra.Blue, gradient.GetColor(1.0));
        }

        [Fact]
        public void GetColor_AtAStop_ReturnsThatStop()
        {
            var gradient = new Gradient();
            gradient.Add(0, ColorBgra.Red);
            gradient.Add(0.5, ColorBgra.Lime);
            gradient.Add(1, ColorBgra.Blue);

            Assert.Equal(ColorBgra.Red, gradient.GetColor(0.0));
            Assert.Equal(ColorBgra.Lime, gradient.GetColor(0.5));
            Assert.Equal(ColorBgra.Blue, gradient.GetColor(1.0));
        }

        [Fact]
        public void GetColor_BetweenStops_BlendsInLinearLight()
        {
            var gradient = new Gradient();
            gradient.SetDefault();

            // 50% linear light is sRGB 188, not 128
            Assert.Equal(ColorBgra.FromBgra(188, 188, 188, 255), gradient.GetColor(0.5));
        }

        [Fact]
        public void GetColor_HardEdge_EachSideUsesItsOwnColor()
        {
            var gradient = new Gradient();
            gradient.Add(0, ColorBgra.Black);
            gradient.Add(0.5, ColorBgra.Red);
            gradient.Add(0.5, ColorBgra.Blue);
            gradient.Add(1, ColorBgra.White);

            ColorBgra left = gradient.GetColor(0.25);
            Assert.True(left.R > 0);
            Assert.Equal(0, left.B);

            ColorBgra right = gradient.GetColor(0.75);
            Assert.Equal(255, right.B);
            Assert.True(right.R > 0 && right.R < 255);

            Assert.Equal(ColorBgra.Blue, gradient.GetColor(0.5));
        }

        [Fact]
        public void GetColor_HardEdgeAtTheEnd_BlendsTowardTheFirstOfThePair()
        {
            var gradient = new Gradient();
            gradient.Add(0, ColorBgra.Black);
            gradient.Add(1, ColorBgra.Red);
            gradient.Add(1, ColorBgra.Blue);

            ColorBgra color = gradient.GetColor(0.5);
            Assert.True(color.R > 0);
            Assert.Equal(0, color.B);
        }

        [Fact]
        public void GetColor_HardEdgeAtTheStart_BlendsFromTheLastOfThePair()
        {
            var gradient = new Gradient();
            gradient.Add(0, ColorBgra.Red);
            gradient.Add(0, ColorBgra.Blue);
            gradient.Add(1, ColorBgra.Black);

            ColorBgra color = gradient.GetColor(0.5);
            Assert.True(color.B > 0);
            Assert.Equal(0, color.R);
        }

        [Fact]
        public void GetColor_SingleStop_IsThatColorEverywhere()
        {
            var gradient = new Gradient();
            gradient.Add(0.3, ColorBgra.Red);

            Assert.Equal(ColorBgra.Red, gradient.GetColor(0.0));
            Assert.Equal(ColorBgra.Red, gradient.GetColor(0.3));
            Assert.Equal(ColorBgra.Red, gradient.GetColor(1.0));
        }

        [Fact]
        public void GetColor_NoStops_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => new Gradient().GetColor(0.5));
        }

        [Fact]
        public void SetPosition_MovingPastNeighbors_ReordersAndReturnsTheNewIndex()
        {
            var gradient = new Gradient();
            gradient.Add(0.0, ColorBgra.Red);
            gradient.Add(0.5, ColorBgra.Lime);
            gradient.Add(1.0, ColorBgra.Blue);

            Assert.Equal(2, gradient.SetPosition(0, 2.0));
            Assert.Equal(new[] { ColorBgra.Lime, ColorBgra.Blue, ColorBgra.Red }, Colors(gradient));

            Assert.Equal(0, gradient.SetPosition(2, -1.0));
            Assert.Equal(new[] { ColorBgra.Red, ColorBgra.Lime, ColorBgra.Blue }, Colors(gradient));
        }

        [Fact]
        public void SetPosition_WithoutPassingANeighbor_KeepsTheIndex()
        {
            var gradient = new Gradient();
            gradient.Add(0.0, ColorBgra.Red);
            gradient.Add(0.5, ColorBgra.Lime);
            gradient.Add(1.0, ColorBgra.Blue);

            Assert.Equal(1, gradient.SetPosition(1, 0.9));
            Assert.Equal(new[] { 0.0, 0.9, 1.0 }, Positions(gradient));
        }

        [Fact]
        public void SetPositions_PairsEachPositionWithTheColorAtThatIndex()
        {
            var gradient = new Gradient();
            gradient.Add(0.0, ColorBgra.Red);
            gradient.Add(0.5, ColorBgra.Lime);
            gradient.Add(1.0, ColorBgra.Blue);

            // every position is past the next color's old position, which is what used to
            // scramble the order
            gradient.SetPositions(new[] { 0.8, 0.85, 0.9 });

            Assert.Equal(new[] { 0.8, 0.85, 0.9 }, Positions(gradient));
            Assert.Equal(new[] { ColorBgra.Red, ColorBgra.Lime, ColorBgra.Blue }, Colors(gradient));
        }

        [Fact]
        public void SetPositions_UnsortedPositions_SortsColorsWithThem()
        {
            var gradient = new Gradient();
            gradient.Add(0.0, ColorBgra.Red);
            gradient.Add(0.5, ColorBgra.Lime);
            gradient.Add(1.0, ColorBgra.Blue);

            gradient.SetPositions(new[] { 0.9, 0.1, 0.5 });

            Assert.Equal(new[] { 0.1, 0.5, 0.9 }, Positions(gradient));
            Assert.Equal(new[] { ColorBgra.Lime, ColorBgra.Blue, ColorBgra.Red }, Colors(gradient));
        }

        [Fact]
        public void SetPositions_EqualPositions_KeepTheirOrder()
        {
            var gradient = new Gradient();
            gradient.Add(0.0, ColorBgra.Red);
            gradient.Add(0.5, ColorBgra.Lime);
            gradient.Add(1.0, ColorBgra.Blue);

            gradient.SetPositions(new[] { 0.5, 0.5, 0.5 });

            Assert.Equal(new[] { ColorBgra.Red, ColorBgra.Lime, ColorBgra.Blue }, Colors(gradient));
        }

        [Fact]
        public void SetPositions_MorePositionsThanColors_AddsBlack()
        {
            var gradient = new Gradient();
            gradient.Add(0.0, ColorBgra.Red);

            gradient.SetPositions(new[] { 0.2, 0.6 });

            Assert.Equal(new[] { 0.2, 0.6 }, Positions(gradient));
            Assert.Equal(new[] { ColorBgra.Red, ColorBgra.Black }, Colors(gradient));
        }

        [Fact]
        public void Reverse_MirrorsPositionsAndOrder()
        {
            var gradient = new Gradient();
            gradient.Add(0.0, ColorBgra.Red);
            gradient.Add(0.25, ColorBgra.Lime);
            gradient.Add(1.0, ColorBgra.Blue);

            gradient.Reverse();

            Assert.Equal(new[] { 0.0, 0.75, 1.0 }, Positions(gradient));
            Assert.Equal(new[] { ColorBgra.Blue, ColorBgra.Lime, ColorBgra.Red }, Colors(gradient));
        }

        [Fact]
        public void Spread_SpacesColorsEvenly()
        {
            var gradient = new Gradient();
            gradient.Add(0.1, ColorBgra.Red);
            gradient.Add(0.15, ColorBgra.Lime);
            gradient.Add(0.2, ColorBgra.Blue);

            gradient.Spread();

            Assert.Equal(new[] { 0.0, 0.5, 1.0 }, Positions(gradient));
            Assert.Equal(new[] { ColorBgra.Red, ColorBgra.Lime, ColorBgra.Blue }, Colors(gradient));
        }

        [Fact]
        public void Spread_SingleColor_IsLeftAlone()
        {
            var gradient = new Gradient();
            gradient.Add(0.3, ColorBgra.Red);

            gradient.Spread();

            Assert.Equal(new[] { 0.3 }, Positions(gradient));
        }

        [Fact]
        public void SetDefault_IsBlackToWhite()
        {
            var gradient = new Gradient();
            gradient.Add(0.3, ColorBgra.Red);

            gradient.SetDefault();

            Assert.Equal(new[] { 0.0, 1.0 }, Positions(gradient));
            Assert.Equal(new[] { ColorBgra.Black, ColorBgra.White }, Colors(gradient));
        }

        [Fact]
        public void Clone_IsIndependentOfTheOriginal()
        {
            var gradient = new Gradient();
            gradient.SetDefault();

            var clone = (Gradient)gradient.Clone();
            clone.SetColor(0, ColorBgra.Red);
            clone.Add(0.5, ColorBgra.Blue);

            Assert.Equal(2, gradient.Count);
            Assert.Equal(ColorBgra.Black, gradient.GetColor(0));
        }

        [Fact]
        public void SetColor_OutOfRange_IsIgnored()
        {
            var gradient = new Gradient();
            gradient.SetDefault();

            gradient.SetColor(-1, ColorBgra.Red);
            gradient.SetColor(2, ColorBgra.Red);

            Assert.Equal(new[] { ColorBgra.Black, ColorBgra.White }, Colors(gradient));
        }
    }
}
