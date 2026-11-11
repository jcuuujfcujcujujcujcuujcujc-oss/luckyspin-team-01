using System.Threading;
using System.Threading.Tasks;
using LuckySpinAPI.Data;
using LuckySpinAPI.Models;

namespace LuckySpinAPI.Services
{
    public enum SpinStatus
    {
        Success,
        InvalidBet,
        UserNotFound,
        InsufficientFunds
    }

    public class SpinResult
    {
        public SpinStatus Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Combination { get; set; } = string.Empty;
        public bool IsWin { get; set; }
        public decimal WinAmount { get; set; }
        public decimal NewBalance { get; set; }
    }

    /// <summary>Игровая логика: баланс игрока и ставки.</summary>
    public class SlotService
    {
        public const decimal StartingBalance = 1000m;

        // Один процесс, одна база в памяти: общий замок делает списание и начисление атомарными (NFR-03).
        private static readonly SemaphoreSlim Gate = new SemaphoreSlim(1, 1);

        private readonly AppDbContext _context;
        private readonly SlotEngine _engine;

        public SlotService(AppDbContext context, SlotEngine engine)
        {
            _context = context;
            _engine = engine;
        }

        /// <summary>
        /// Возвращает баланс. Для нового userId создаёт игрока со стартовым балансом
        /// (в MVP регистрации нет). Единственный метод, создающий игрока.
        /// </summary>
        public async Task<decimal> GetOrCreateBalanceAsync(int userId)
        {
            await Gate.WaitAsync();
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    user = new User { Id = userId, Balance = StartingBalance };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }
                return user.Balance;
            }
            finally
            {
                Gate.Release();
            }
        }

        public async Task<SpinResult> SpinAsync(int userId, decimal betAmount)
        {
            // Ставка должна быть положительной: иначе отрицательная ставка увеличивала бы баланс (FR-04).
            if (betAmount <= 0)
            {
                return Fail(SpinStatus.InvalidBet, "Ставка должна быть больше нуля.");
            }

            await Gate.WaitAsync();
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return Fail(SpinStatus.UserNotFound, "Игрок не найден. Сначала запросите баланс, чтобы создать игрока.");
                }

                if (user.Balance < betAmount)
                {
                    return Fail(SpinStatus.InsufficientFunds, "Недостаточно кредитов на балансе.");
                }

                var outcome = _engine.Spin();
                decimal winAmount = _engine.CalculateWin(betAmount, outcome.IsWin);

                user.Balance = user.Balance - betAmount + winAmount;
                await _context.SaveChangesAsync();

                return new SpinResult
                {
                    Status = SpinStatus.Success,
                    Combination = outcome.Combination,
                    IsWin = outcome.IsWin,
                    WinAmount = winAmount,
                    NewBalance = user.Balance
                };
            }
            finally
            {
                Gate.Release();
            }
        }

        private static SpinResult Fail(SpinStatus status, string message)
        {
            return new SpinResult { Status = status, Message = message };
        }
    }
}
