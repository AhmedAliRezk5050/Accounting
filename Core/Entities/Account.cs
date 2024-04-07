namespace Core.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public long Code { get; set; }

        public string EnglishName { get; set; } = null!;

        public string ArabicName { get; set; } = null!;

        public string Currency { get; set; } = null!;

        public bool IsMain { get; set; }

        public int AccountLevel { get; set; }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }

        public decimal Balance { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public int? ParentId { get; set; }
        public Account? Parent { get; set; }

        public IList<Account> SubAccounts { get; set; } = null!;
        public IList<EntryDetails> EntryDetails { get; set; } = null!;

        public Customer Customer { get; set; } = null!;
        public Supplier Supplier { get; set; } = null!;
    }
}