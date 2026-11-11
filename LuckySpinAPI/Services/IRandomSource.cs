namespace LuckySpinAPI.Services
{
    /// <summary>Источник случайных чисел. Вынесен в интерфейс, чтобы в тестах подменять случайность.</summary>
    public interface IRandomSource
    {
        /// <summary>Возвращает число от 0 (включительно) до maxExclusive (не включая).</summary>
        int Next(int maxExclusive);
    }
}
