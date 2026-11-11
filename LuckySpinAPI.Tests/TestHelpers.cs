using System;
using System.Collections.Generic;
using System.Linq;
using LuckySpinAPI.Data;
using LuckySpinAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace LuckySpinAPI.Tests
{
    /// <summary>Подменяет случайность: возвращает заранее заданные индексы символов по кругу.</summary>
    public class FakeRandomSource : IRandomSource
    {
        private readonly int[] _values;
        private int _position;

        public FakeRandomSource(params int[] values)
        {
            _values = values;
        }

        public int Next(int maxExclusive)
        {
            lock (_values)
            {
                int value = _values[_position % _values.Length];
                _position++;
                return value % maxExclusive;
            }
        }
    }

    public static class TestHelpers
    {
        // Индексы символов в SlotEngine.Symbols: 0 = 🍒, 1 = 🍋, 2 = 777
        public static readonly int[] WinningRolls = { 0, 0, 0 };
        public static readonly int[] LosingRolls = { 0, 1, 2 };

        public static DbContextOptions<AppDbContext> NewDatabase()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("test-" + Guid.NewGuid())
                .Options;
        }

        public static SlotService NewService(DbContextOptions<AppDbContext> options, params int[] rolls)
        {
            var context = new AppDbContext(options);
            var engine = new SlotEngine(new FakeRandomSource(rolls));
            return new SlotService(context, engine);
        }

        public static decimal ReadBalance(DbContextOptions<AppDbContext> options, int userId)
        {
            using (var context = new AppDbContext(options))
            {
                return context.Users.Find(userId).Balance;
            }
        }

        public static int CountUsers(DbContextOptions<AppDbContext> options)
        {
            using (var context = new AppDbContext(options))
            {
                return context.Users.Count();
            }
        }
    }
}
