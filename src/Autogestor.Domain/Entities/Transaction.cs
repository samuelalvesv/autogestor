using Autogestor.Domain.Enums;

namespace Autogestor.Domain.Entities;

public sealed class Transaction : TenantEntity
{
    public string Title { get; private set; } = string.Empty;
    public ETransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category? Category { get; private set; }

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
        Validate(title: title, type: type, amount: amount, categoryId: categoryId);
        return new Transaction(title: title, type: type, amount: amount, categoryId: categoryId);
    }

    public void Update(string title, ETransactionType type, decimal amount, Guid categoryId)
    {
        Validate(title: title, type: type, amount: amount, categoryId: categoryId);

        Title = title;
        Type = type;
        Amount = amount;
        CategoryId = categoryId;
    }

    private static void Validate(string title, ETransactionType type, decimal amount, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException(message: "O título da transação não pode ser vazio.", paramName: nameof(title));

        if (!Enum.IsDefined(type))
            throw new ArgumentException(message: "Tipo de transação inválido.", paramName: nameof(type));

        if (amount <= 0)
            throw new ArgumentException(message: "O valor da transação deve ser maior que zero.", paramName: nameof(amount));

        if (categoryId == Guid.Empty)
            throw new ArgumentException(message: "Categoria inválida.", paramName: nameof(categoryId));
    }
}
