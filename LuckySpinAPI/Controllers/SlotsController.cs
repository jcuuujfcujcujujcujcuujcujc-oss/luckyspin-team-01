using System.Threading.Tasks;
using LuckySpinAPI.Models;
using LuckySpinAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LuckySpinAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SlotsController : ControllerBase
    {
        private readonly SlotService _service;

        public SlotsController(SlotService service)
        {
            _service = service;
        }

        // GET /api/slots/balance/1 — баланс; для нового игрока создаётся запись с 1000 кредитов
        [HttpGet("balance/{userId}")]
        public async Task<IActionResult> GetBalance(int userId)
        {
            decimal balance = await _service.GetOrCreateBalanceAsync(userId);
            return Ok(new { balance });
        }

        // POST /api/slots/spin — ставка и вращение
        [HttpPost("spin")]
        public async Task<IActionResult> Spin([FromBody] SpinRequest request)
        {
            var result = await _service.SpinAsync(request.UserId, request.BetAmount);

            switch (result.Status)
            {
                case SpinStatus.Success:
                    return Ok(new
                    {
                        combination = result.Combination,
                        newBalance = result.NewBalance,
                        isWin = result.IsWin,
                        winAmount = result.WinAmount
                    });
                case SpinStatus.UserNotFound:
                    return NotFound(new { message = result.Message });
                default:
                    return BadRequest(new { message = result.Message });
            }
        }
    }
}
