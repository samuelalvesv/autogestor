namespace Autogestor.Domain.Entities;

public sealed class Category : AuditableEntity
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }

    private Category() { }

    private Category(string title, string description, Guid userId)
    {
        Title = title;
        Description = description;
        UserId = userId;
    }

    public static Category Create(string title, string description, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("O título da categoria não pode ser vazio.", nameof(title));
        }

        return string.IsNullOrWhiteSpace(description)
            ? throw new ArgumentException("A descrição da categoria não pode ser vazia.", nameof(description))
            : userId == Guid.Empty
            ? throw new ArgumentException("Usuário inválido.", nameof(userId))
            : new Category(title, description, userId);
    }
}
