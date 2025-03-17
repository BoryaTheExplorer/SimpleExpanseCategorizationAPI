



namespace ExpanseCategorizationAPI.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}