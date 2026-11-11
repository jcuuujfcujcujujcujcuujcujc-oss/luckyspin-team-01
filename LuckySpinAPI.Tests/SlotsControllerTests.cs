using System.Text.Json;
using System.Threading.Tasks;
using LuckySpinAPI.Controllers;
using LuckySpinAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace LuckySpinAPI.Tests
{
    /// <summary>Проверяют HTTP-коды и формат ошибок (NFR-04): JSON с полем message.</summary>
    public class SlotsControllerTests
    {
        private static SlotsController NewController(params int[] rolls)
        {
            var options = TestHelpers.NewDatabase();
            return new SlotsController(TestHelpers.NewService(options, rolls));
        }

        [Fact]
        public async Task Spin_NegativeBet_Returns400WithMessage()
        {
            var controller = NewController(TestHelpers.WinningRolls);
            await controller.GetBalance(1);

            var result = await controller.Spin(new SpinRequest { UserId = 1, BetAmount = -100 });

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Ставка должна быть больше нуля.", JsonSerializer.Serialize(bad.Value));
        }

        [Fact]
        public async Task Spin_UnknownPlayer_Returns404WithMessage()
        {
            var controller = NewController(TestHelpers.WinningRolls);

            var result = await controller.Spin(new SpinRequest { UserId = 7, BetAmount = 10 });

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("message", JsonSerializer.Serialize(notFound.Value));
        }

        [Fact]
        public async Task Spin_ValidBet_Returns200WithAllFields()
        {
            var controller = NewController(TestHelpers.LosingRolls);
            await controller.GetBalance(1);

            var result = await controller.Spin(new SpinRequest { UserId = 1, BetAmount = 100 });

            var ok = Assert.IsType<OkObjectResult>(result);
            string json = JsonSerializer.Serialize(ok.Value);
            Assert.Contains("combination", json);
            Assert.Contains("newBalance", json);
            Assert.Contains("isWin", json);
            Assert.Contains("winAmount", json);
        }

        [Fact]
        public async Task GetBalance_NewPlayer_Returns200WithBalance1000()
        {
            var controller = NewController(TestHelpers.LosingRolls);

            var result = await controller.GetBalance(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("1000", JsonSerializer.Serialize(ok.Value));
        }
    }
}
