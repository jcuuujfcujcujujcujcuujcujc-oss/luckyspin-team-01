namespace LuckySpinAPI.Models
{
    /// <summary>Входящий JSON запроса ставки.</summary>
    public class SpinRequest
    {
        public int UserId { get; set; }
        public decimal BetAmount { get; set; }
    }
}
