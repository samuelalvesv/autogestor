using Autogestor.Domain.Enums;

namespace Autogestor.Domain.Entities;

public sealed class Transaction : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public ETransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    private Transaction() { }

    private Transaction(string name, ETransactionType type, decimal amount, Guid categoryId)
    {
        Name = name;
        Type = type;
        Amount = amount;
        CategoryId = categoryId;
    }

    public static Transaction Create(string name, ETransactionType type, decimal amount, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Transaction name cannot be empty.", nameof(name));
        if (amount <= 0)
            throw new ArgumentException("Transaction amount must be greater than zero.", nameof(amount));
        if (categoryId == Guid.Empty)
            throw new ArgumentException("Category ID must be a valid GUID.", nameof(categoryId));

        return new Transaction(name, type, amount, categoryId);
    }
}