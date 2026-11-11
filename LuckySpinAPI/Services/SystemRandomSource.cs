using System;

namespace LuckySpinAPI.Services
{
    public class SystemRandomSource : IRandomSource
    {
        private readonly Random _random = new Random();

        public int Next(int maxExclusive)
        {
            // Random не потокобезопасен, поэтому блокируем обращение.
            lock (_random)
            {
                return _random.Next(maxExclusive);
            }
        }
    }
}
