namespace LuckySpinAPI.Services
{
    /// <summary>Результат одного вращения барабанов.</summary>
    public class SpinOutcome
    {
        public string Combination { get; set; } = string.Empty;
        public bool IsWin { get; set; }
    }

    /// <summary>Правила игры: единственное место, где определены символы и выплаты (FR-03).</summary>
    public class SlotEngine
    {
        /// <summary>Множитель выигрыша при трёх одинаковых символах.</summary>
        public const decimal WinMultiplier = 5m;

        public static readonly string[] Symbols = { "🍒", "🍋", "777" };

        private readonly IRandomSource _random;

        public SlotEngine(IRandomSource random)
        {
            _random = random;
        }

        public SpinOutcome Spin()
        {
            string s1 = Symbols[_random.Next(Symbols.Length)];
            string s2 = Symbols[_random.Next(Symbols.Length)];
            string s3 = Symbols[_random.Next(Symbols.Length)];

            return new SpinOutcome
            {
                Combination = $"{s1}|{s2}|{s3}",
                IsWin = s1 == s2 && s2 == s3
            };
        }

        /// <summary>Сумма выигрыша: ставка × 5 при победе, иначе 0.</summary>
        public decimal CalculateWin(decimal betAmount, bool isWin)
        {
            return isWin ? betAmount * WinMultiplier : 0m;
        }
    }
}
