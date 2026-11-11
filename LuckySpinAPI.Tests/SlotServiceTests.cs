using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuckySpinAPI.Services;
using Xunit;

namespace LuckySpinAPI.Tests
{
    public class SlotServiceTests
    {
        // ---- FR-01: баланс ----

        [Fact]
        public async Task GetBalance_NewPlayer_CreatesPlayerWithStartingBalance()
        {
            var options = TestHelpers.NewDatabase();
            var service = TestHelpers.NewService(options, TestHelpers.LosingRolls);

            decimal balance = await service.GetOrCreateBalanceAsync(1);

            Assert.Equal(1000m, balance);
            Assert.Equal(1, TestHelpers.CountUsers(options));
        }

        [Fact]
        public async Task GetBalance_ExistingPlayer_DoesNotCreateDuplicate()
        {
            var options = TestHelpers.NewDatabase();
            var service = TestHelpers.NewService(options, TestHelpers.LosingRolls);

            await service.GetOrCreateBalanceAsync(1);
            await service.GetOrCreateBalanceAsync(1);

            Assert.Equal(1, TestHelpers.CountUsers(options));
        }

        // ---- FR-02, FR-03: ставка и выигрыш ----

        [Fact]
        public async Task Spin_Win_PaysFiveTimesBet()
        {
            var options = TestHelpers.NewDatabase();
            var service = TestHelpers.NewService(options, TestHelpers.WinningRolls);
            await service.GetOrCreateBalanceAsync(1);

            var result = await service.SpinAsync(1, 100);

            Assert.Equal(SpinStatus.Success, result.Status);
            Assert.True(result.IsWin);
            Assert.Equal(500m, result.WinAmount);
            Assert.Equal(1400m, result.NewBalance); // 1000 - 100 + 500
            Assert.Equal(1400m, TestHelpers.ReadBalance(options, 1));
        }

        [Fact]
        public async Task Spin_Loss_DeductsBetOnly()
        {
            var options = TestHelpers.NewDatabase();
            var service = TestHelpers.NewService(options, TestHelpers.LosingRolls);
            await service.GetOrCreateBalanceAsync(1);

            var result = await service.SpinAsync(1, 100);

            Assert.Equal(SpinStatus.Success, result.Status);
            Assert.False(result.IsWin);
            Assert.Equal(0m, result.WinAmount);
            Assert.Equal(900m, result.NewBalance);
        }

        // ---- FR-04: некорректные ставки (негативные сценарии US-03) ----

        [Theory]
        [InlineData(-100)]
        [InlineData(0)]
        public async Task Spin_NonPositiveBet_IsRejectedAndBalanceUnchanged(decimal bet)
        {
            var options = TestHelpers.NewDatabase();
            var service = TestHelpers.NewService(options, TestHelpers.WinningRolls);
            await service.GetOrCreateBalanceAsync(1);

            var result = await service.SpinAsync(1, bet);

            Assert.Equal(SpinStatus.InvalidBet, result.Status);
            Assert.Equal("Ставка должна быть больше нуля.", result.Message);
            Assert.Equal(1000m, TestHelpers.ReadBalance(options, 1));
        }

        [Fact]
        public async Task Spin_BetAboveBalance_IsRejectedAndBalanceUnchanged()
        {
            var options = TestHelpers.NewDatabase();
            var service = TestHelpers.NewService(options, TestHelpers.WinningRolls);
            await service.GetOrCreateBalanceAsync(1);

            var result = await service.SpinAsync(1, 1001);

            Assert.Equal(SpinStatus.InsufficientFunds, result.Status);
            Assert.Equal(1000m, TestHelpers.ReadBalance(options, 1));
        }

        [Fact]
        public async Task Spin_BetEqualToBalance_IsAllowed()
        {
            var options = TestHelpers.NewDatabase();
            var service = TestHelpers.NewService(options, TestHelpers.LosingRolls);
            await service.GetOrCreateBalanceAsync(1);

            var result = await service.SpinAsync(1, 1000);

            Assert.Equal(SpinStatus.Success, result.Status);
            Assert.Equal(0m, result.NewBalance);
        }

        [Fact]
        public async Task Spin_UnknownPlayer_ReturnsNotFoundAndDoesNotCreatePlayer()
        {
            var options = TestHelpers.NewDatabase();
            var service = TestHelpers.NewService(options, TestHelpers.WinningRolls);

            var result = await service.SpinAsync(42, 10);

            Assert.Equal(SpinStatus.UserNotFound, result.Status);
            Assert.Equal(0, TestHelpers.CountUsers(options));
        }

        // ---- NFR-03: параллельные ставки ----

        [Fact]
        public async Task Spin_ParallelBets_BalanceNeverGoesNegative()
        {
            var options = TestHelpers.NewDatabase();
            await TestHelpers.NewService(options, TestHelpers.LosingRolls).GetOrCreateBalanceAsync(1);

            // 20 параллельных ставок по 50 при балансе 1000 (проигрыш всегда): проходят ровно 20, баланс 0.
            // Затем ещё 5 ставок должны быть отклонены.
            var tasks = new List<Task<SpinResult>>();
            for (int i = 0; i < 25; i++)
            {
                var service = TestHelpers.NewService(options, TestHelpers.LosingRolls);
                tasks.Add(Task.Run(() => service.SpinAsync(1, 50)));
            }
            var results = await Task.WhenAll(tasks);

            Assert.Equal(20, results.Count(r => r.Status == SpinStatus.Success));
            Assert.Equal(5, results.Count(r => r.Status == SpinStatus.InsufficientFunds));
            Assert.Equal(0m, TestHelpers.ReadBalance(options, 1));
        }
    }
}
