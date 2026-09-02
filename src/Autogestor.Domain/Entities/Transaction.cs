using Autogestor.Domain.Enums;

namespace Autogestor.Domain.Entities;

public sealed class Transaction : AuditableEntity
{
    public string Title { get; private set; } = string.Empty;
    public ETransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    private Transaction() { }

    private Transaction(string title, ETransactionType type, decimal amount, Guid categoryId)
    {
        Title = title;
        Type = type;
        Amount = amount;
        CategoryId = categoryId;
    }

    public static Transaction Create(string title, ETransactionType type, decimal amount, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("O título da transação não pode ser vazio.", nameof(title));
        }

        return amount <= 0
            ? throw new ArgumentException("O valor da transação deve ser maior que zero.", nameof(amount))
            : categoryId == Guid.Empty
            ? throw new ArgumentException("Categoria inválida.", nameof(categoryId))
            : new Transaction(title, type, amount, categoryId);
    }
}
