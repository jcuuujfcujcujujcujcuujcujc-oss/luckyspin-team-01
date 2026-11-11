using LuckySpinAPI.Services;
using Xunit;

namespace LuckySpinAPI.Tests
{
    public class SlotEngineTests
    {
        [Fact]
        public void Spin_ThreeEqualSymbols_IsWin()
        {
            var engine = new SlotEngine(new FakeRandomSource(TestHelpers.WinningRolls));

            var outcome = engine.Spin();

            Assert.True(outcome.IsWin);
            Assert.Equal("🍒|🍒|🍒", outcome.Combination);
        }

        [Fact]
        public void Spin_DifferentSymbols_IsNotWin()
        {
            var engine = new SlotEngine(new FakeRandomSource(TestHelpers.LosingRolls));

            var outcome = engine.Spin();

            Assert.False(outcome.IsWin);
            Assert.Equal("🍒|🍋|777", outcome.Combination);
        }

        [Theory]
        [InlineData(100, true, 500)]
        [InlineData(1, true, 5)]
        [InlineData(100, false, 0)]
        public void CalculateWin_UsesMultiplierFive(decimal bet, bool isWin, decimal expected)
        {
            var engine = new SlotEngine(new FakeRandomSource(0));

            Assert.Equal(expected, engine.CalculateWin(bet, isWin));
        }
    }
}
